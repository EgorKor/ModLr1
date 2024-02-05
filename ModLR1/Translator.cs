using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
        private Stack stack;                //Стэк
        private string currentInfixSequence;//Текущее инфиксное выражение
        private int infixPointer = 0;       //Указатель на обрабатываемый символ
        /*Константы операций транслятора*/
        private const int OP_PUSH = 1;
        private const int OP_POP = 2;
        private const int OP_DEL_PAR = 3;
        private const int OP_SUCCESS = 4;
        private const int OP_ERR = 5;
        private const int OP_OUTPUT = 6;
        /*таблица принятия реешений*/
        private int[,] actionTable;
        /*Константы арифметических операция*/
        /*Нужны для организации доступа к таблице принятия решений*/
        private const int ARF_EMPTY = 0;
        private const int ARF_PLUS = 1;
        private const int ARF_MINUS = 2;
        private const int ARF_MULT = 3;
        private const int ARF_DIV = 4;
        private const int ARF_OPEN_PAR = 5;
        private const int ARF_CLOSE_PAR = 6;
        private const int ARF_SYMBOL = 7;


        private Dictionary<int, string> dic;

        private const string OP_RES_SUCCESS = "SUCCESS";
        private const string OP_RES_ERR = "ERROR";
        private const string OP_RES_DEL_PAR = "DELPAR";
        private const string OP_RES_PUSH = "PUSH";


        public Translator()
        {
            stack = new Stack();
            actionTable = new int[,]
            {
                {4,1,1,1,1,1,5,6},
                {2,2,2,1,1,1,2,6},
                {2,2,2,1,1,1,2,6},
                {2,2,2,2,2,1,2,6},
                {2,2,2,2,2,1,2,6},
                {5,1,1,1,1,1,3,6}
            };
            
        }
        public Translator(string infixSequence)
        {
            currentInfixSequence = infixSequence;
            stack = new Stack();
            actionTable = new int[,]
            {
                {4,1,1,1,1,1,5,6},
                {2,2,2,1,1,1,2,6},
                {2,2,2,1,1,1,2,6},
                {2,2,2,2,2,1,2,6},
                {2,2,2,2,2,1,2,6},
                {5,1,1,1,1,1,3,6}
            };
        }

        public void changeInfixSequence(string newInfixSequence)
        {
            infixPointer = 0;
            currentInfixSequence = newInfixSequence;
        }


        public string translateInfix()
        {
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
                    default:
                        {
                            sb.Append(operationResult);
                            break;
                        }
                }
            }
            return sb.ToString();
        }

        public string processTranslatorOperation(int OP_CODE)
        {
            switch (OP_CODE)
            {
                case OP_PUSH:
                    {
                        stack.Push(currentInfixSequence[infixPointer++].ToString());
                        return OP_RES_PUSH;
                    }
                case OP_POP:
                    return stack.Pop();
                case OP_OUTPUT:
                    return currentInfixSequence[infixPointer++].ToString();
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
            if (infixPointer == currentInfixSequence.Length)
                return ARF_EMPTY;
            switch (currentInfixSequence[infixPointer].ToString())
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
                default: 
                    return ARF_SYMBOL;
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
                default: throw new Exception("Illegal argument exception: unsupported symbol in Translator!");
            }
        }
        //Возвращает true если есть символы которые нужно обработать в
        //инфиксной последовательности или в стэке
        public bool hasNext()
        {
            return infixPointer != currentInfixSequence.Length || !stack.isEmpty();
        }



    }
}
