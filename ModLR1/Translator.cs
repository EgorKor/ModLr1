using Microsoft.SqlServer.Server;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModLR1
{
    /*
        Объект класса Translator -> обработай инфиксное выражение
        -> Translator хранит инфиксное выражение -> 
        * Translator может выдать результирующую строку 
        при помощи метода translateInfix
        * Translator может посимвольно обрабатывать инфиксную
        строку используя методы next, hasNext, 
     */

    public class Translator
    {
        private Stack stack = new Stack();  //Стэк
        private string currentInfixSequence;//Текущее инфиксное выражение
        private string currentInfixSequenceEncoded;
        private int infixPointer = 0;       //Указатель на обрабатываемый символ
        /*Константы операций транслятора*/
        private const int OP_PUSH = 1;
        private const int OP_POP = 2;
        private const int OP_DEL_PAR = 3;
        private const int OP_SUCCESS = 4;
        private const int OP_ERR_CLOSE = 5;
        private const int OP_ERR_OPEN = 6;
        private const int OP_ERR_FUNC = 7;
        private const int OP_OUTPUT = 8;

        //Таблица принятия решений
        private int[,] actionTable = new int[,]
            {   //     |$          |+      |-      |*      |/      |^      |(      |F           |)           |P
                /* $ */{OP_SUCCESS ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_ERR_CLOSE,OP_OUTPUT},
                /* + */{OP_POP     ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_POP      ,OP_OUTPUT},
                /* - */{OP_POP     ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_POP      ,OP_OUTPUT},
                /* * */{OP_POP     ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_POP      ,OP_OUTPUT},
                /* / */{OP_POP     ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_POP      ,OP_OUTPUT},
                /* ^ */{OP_POP     ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH     ,OP_POP      ,OP_OUTPUT},
                /* ( */{OP_ERR_OPEN,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH     ,OP_DEL_PAR  ,OP_OUTPUT},
                /* F */{OP_POP     ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_ERR_FUNC ,OP_POP      ,OP_OUTPUT}
            };
        /*Константы арифметических операция*/
        /*Нужны для организации доступа к таблице принятия решений*/
        private const int ARF_EMPTY = 0;
        private const int ARF_PLUS = 1;
        private const int ARF_MINUS = 2;
        private const int ARF_MULT = 3;
        private const int ARF_DIV = 4;
        private const int ARF_POW = 5;
        private const int ARF_OPEN_PAR = 6;
        private const int ARF_FUNCTION = 7;
        private const int ARF_CLOSE_PAR = 8;
        private const int ARF_VARIABLE = 9;

        //словарь кодирования
        private Dictionary<string, string> funcionEncodeDictionary = new Dictionary<string, string>()
        {
            {"sin","в"},
            {"cos","г"},
            {"ln","и"}
        };

        //словарь декодирования
        private Dictionary<string, string> functionDecodeDictionary = new Dictionary<string, string>()
        {
            {"в","sin"},
            {"г","cos"},
            {"и","ln"}
        };

        //словарь кодов символов и самих символов
        private Dictionary<int, string> arfCodeDictionary = new Dictionary<int, string>()
        {
            {ARF_EMPTY, ""},
            {ARF_DIV,"/"},
            {ARF_MULT,"*"},
            {ARF_PLUS,"+"},
            {ARF_MINUS,"-"},
            {ARF_POW,"^"},
            {ARF_CLOSE_PAR,")"},
            {ARF_OPEN_PAR,"("}
        };


        public const string OP_RES_SUCCESS = "SUCCESS";
        public const string OP_RES_ERR_CLOSE = "ERROR_CLOSE_PAR";
        public const string OP_RES_ERR_OPEN = "ERROR_OPEN_PAR";
        public const string OP_RES_ERR_FUNC = "ERROR_FUNC";
        public const string OP_RES_DEL_PAR = "DELPAR";
        public const string OP_RES_PUSH = "PUSH";

        public Translator(){}
        public Translator(string infixSequence)
        {
            changeInfixSequence(infixSequence);
        }

        //Метод меняющий обрабатываемую строку транслятором
        public void changeInfixSequence(string newInfixSequence)
        {
            clearStackAndInfixPointer();
            currentInfixSequence = newInfixSequence;
            currentInfixSequenceEncoded = encodeInfix(currentInfixSequence);
        }

        //Метод кодирования инфиксной строки
        private string encodeInfix(string infix)
        {
            string encoded = infix;
            foreach (KeyValuePair<string, string> entry in funcionEncodeDictionary)
            {
                encoded = encoded.Replace(entry.Key, entry.Value);
            }
            return encoded;
        }

        //Метод декодирования постфиксной строки
        private string decodePostfix(string postfix)
        {
            string decoded = postfix;
            foreach (KeyValuePair<string, string> entry in functionDecodeDictionary)
            {
                decoded = decoded.Replace(entry.Key, entry.Value);
            }
            return decoded;
        }


        //Моделирует обработку инфиксной строки и возвращает постфиксную строку
        public string translateInfix()
        {
            clearStackAndInfixPointer();
            StringBuilder sb = new StringBuilder();

            while (hasNext())
            {
                int beforeOperationCode = nextInfixCode();
                string operationResult = processTranslatorOperation(currentOperation());
                int afterOperationCode = nextInfixCode();
                if (currentOperation() != OP_POP) {
                    validateInfix(beforeOperationCode, afterOperationCode);
                }
                switch (operationResult)
                {
                    case OP_RES_ERR_OPEN:
                        throw new SyntaxValidationException("Syntax error exception: there is no pair for closing parenthesis )!");
                    case OP_RES_ERR_CLOSE:
                        throw new SyntaxValidationException("Syntax error exception: there is no pair for open parenthesis (!");
                    case OP_RES_ERR_FUNC:
                        throw new SyntaxValidationException($"Syntax error exception: function parenthesis structure problem!\n" +
                            $"func #1 = {functionDecodeDictionary[stack.Poll()]} func #2 = {functionDecodeDictionary[currentInfixSequenceEncoded[infixPointer].ToString()]}"); ;
                    case OP_RES_PUSH:
                    case OP_RES_DEL_PAR:
                    case OP_RES_SUCCESS:
                        break;
                    default:
                        {
                            sb.Append(operationResult);
                            break;
                        }
                }
            }
            return decodePostfix(sb.ToString());
        }

        public void validateInfix(int firstCode, int secondCode)
        {
            /*Выражение Ошибка …A +* d… Подряд два символа операций 
             * …A d… Подряд две переменные 
             * …A (… Между переменной и скобкой отсутствует символ операции */
            if ((firstCode == ARF_MINUS ||
                firstCode == ARF_PLUS   ||
                firstCode == ARF_MULT   ||
                firstCode == ARF_POW    ||
                firstCode == ARF_DIV)   &&
                (secondCode == ARF_MINUS ||
                secondCode == ARF_PLUS ||
                secondCode == ARF_MULT ||
                secondCode == ARF_POW   ||
                secondCode == ARF_DIV))
            {
                throw new SyntaxValidationException($"Syntax Validation Exception: two arifmetical operations in row! operation #1 = {arfCodeDictionary[firstCode]} operation #2 = {arfCodeDictionary[secondCode]}");
            }
            if (firstCode == ARF_VARIABLE && secondCode == ARF_VARIABLE)
            {
                throw new SyntaxValidationException($"Syntax Validation Exception: two variables in row! var #1 = {currentInfixSequenceEncoded[infixPointer]} var #2 = {currentInfixSequenceEncoded[infixPointer - 1]}");
            }
            if (firstCode == ARF_VARIABLE && secondCode==ARF_OPEN_PAR)
            {
                throw new SyntaxValidationException($"Syntax Validation Exception: between variable and opening parthese no operation! var = {currentInfixSequenceEncoded[infixPointer - 1]}");
            }
        }

        public string processTranslatorOperation(int OP_CODE)
        {
            switch (OP_CODE)
            {
                case OP_PUSH:
                    {
                        stack.Push(currentInfixSequenceEncoded[infixPointer++].ToString());
                        return OP_RES_PUSH;
                    }
                case OP_POP:
                    return stack.Pop();
                case OP_OUTPUT:
                    return currentInfixSequenceEncoded[infixPointer++].ToString();
                case OP_ERR_OPEN:
                    return OP_RES_ERR_OPEN;
                case OP_ERR_CLOSE:
                    return OP_RES_ERR_CLOSE;
                case OP_ERR_FUNC:
                    return OP_RES_ERR_FUNC;
                case OP_DEL_PAR:
                    {
                        stack.Pop();
                        infixPointer++;
                        return OP_RES_DEL_PAR;
                    }
                case OP_SUCCESS:
                    return OP_RES_SUCCESS;
                default:
                    throw new Exception("Illegal argument exception: unsupported operation in Translator!");

            }
        }


        //Возвращает результат трансляции инфиксного символа
        public int currentOperation()
        {
            return actionTable[nextStackCode(), nextInfixCode()];
        }

        //Читает элемент строки на который указывает указатель
        public int nextInfixCode()
        {
            if (infixPointer == currentInfixSequenceEncoded.Length)
                return ARF_EMPTY;
            switch (currentInfixSequenceEncoded[infixPointer].ToString())
            {
                case "+":
                    return ARF_PLUS;
                case "-":
                    return ARF_MINUS;
                case "*":
                    return ARF_MULT;
                case "/":
                    return ARF_DIV;
                case "(":
                    return ARF_OPEN_PAR;
                case ")":
                    return ARF_CLOSE_PAR;
                case "^":
                    return ARF_POW;
                default:
                    {
                        if (Regex.IsMatch(currentInfixSequenceEncoded[infixPointer].ToString(),"[а-яА-Я]"))
                            return ARF_FUNCTION;
                        return ARF_VARIABLE;
                    }
            }
        }
        //Читает верхний элемент стэка
        public int nextStackCode()
        {
            if (stack.isEmpty())
                return ARF_EMPTY;
            switch (stack.Poll())
            {
                case "+":
                    return ARF_PLUS;
                case "-":
                    return ARF_MINUS;
                case "*":
                    return ARF_MULT;
                case "/":
                    return ARF_DIV;
                case "(":
                    return ARF_OPEN_PAR;
                case ")":
                    return ARF_CLOSE_PAR;
                case "^":
                    return ARF_POW;
                default:
                    {
                        if (Regex.IsMatch(stack.Poll(), "[а-яА-Я]"))
                            return ARF_FUNCTION;
                        throw new Exception("Illegal argument exception: unsupported symbol in Translator!");
                    };
            }
        }
        //Возвращает true если есть символы которые нужно обработать в
        //инфиксной последовательности или в стэке
        public bool hasNext()
        {
            return infixPointer != currentInfixSequenceEncoded.Length || !stack.isEmpty();
        }

        public void clearStackAndInfixPointer()
        {
            infixPointer = 0;
            stack.Clear();
        }



    }
}
