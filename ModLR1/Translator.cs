using Microsoft.SqlServer.Server;
using System;
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
        private const int OP_ERR = 5;
        private const int OP_OUTPUT = 6;

        //Таблица принятия решений
        private int[,] actionTable = new int[,]
            {   //     |$         |+      |-      |*      |/      |^      |(      |F      |)         |P
                /* $ */{OP_SUCCESS,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_ERR    ,OP_OUTPUT},
                /* + */{OP_POP    ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_POP    ,OP_OUTPUT},
                /* - */{OP_POP    ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_POP    ,OP_OUTPUT},
                /* * */{OP_POP    ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_POP    ,OP_OUTPUT},
                /* / */{OP_POP    ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_PUSH,OP_POP    ,OP_OUTPUT},
                /* ^ */{OP_POP    ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_PUSH,OP_POP    ,OP_OUTPUT},
                /* ( */{OP_ERR    ,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_PUSH,OP_DEL_PAR,OP_OUTPUT},
                /* F */{OP_POP    ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_POP ,OP_PUSH,OP_ERR ,OP_POP    ,OP_OUTPUT}
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


        private Dictionary<string, string> funcionEncodeDictionary = new Dictionary<string, string>()
        {
            {"sin","в"},
            {"cos","г"},
            {"ln","и"}
        };

        private Dictionary<string, string> functionDecodeDictionary = new Dictionary<string, string>()
        {
            {"в","sin"},
            {"г","cos"},
            {"и","ln"}
        };

        private const string OP_RES_SUCCESS = "SUCCESS";
        private const string OP_RES_ERR = "ERROR";
        private const string OP_RES_DEL_PAR = "DELPAR";
        private const string OP_RES_PUSH = "PUSH";


        private Regex functionRegex = new Regex("[а-яА-Я]");

        public Translator(){}
        public Translator(string infixSequence)
        {
            changeInfixSequence(infixSequence);
        }

        public void changeInfixSequence(string newInfixSequence)
        {
            clearStackAndInfixPointer();
            currentInfixSequence = newInfixSequence;
            currentInfixSequenceEncoded = encodeInfix(currentInfixSequence);
        }

        private string encodeInfix(string infix)
        {
            string encoded = infix;
            foreach (KeyValuePair<string, string> entry in funcionEncodeDictionary)
            {
                encoded = encoded.Replace(entry.Key, entry.Value);
            }
            return encoded;
        }

        private string decodePostfix(string postfix)
        {
            string encoded = postfix;
            foreach (KeyValuePair<string, string> entry in functionDecodeDictionary)
            {
                encoded = encoded.Replace(entry.Key, entry.Value);
            }
            return encoded;
        }


        public string translateInfix()
        {
            clearStackAndInfixPointer();
            StringBuilder sb = new StringBuilder();
            while (hasNext())
            {
                string operationResult = processTranslatorOperation(currentOperation());
                switch (operationResult)
                {
                    case OP_RES_ERR:
                        throw new Exception("Syntax error exception: while translating syntax error was occured!");
                    case OP_RES_PUSH:
                        break;
                    case OP_RES_DEL_PAR:
                        break;
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
                case OP_ERR:
                    return OP_RES_ERR;
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
                        {
                            return ARF_FUNCTION;
                        }
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
