using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ModLR1
{
    public class PostfixCalculator
    {
        private Stack<double> stack;//стек для вычислений
        private string postfix;//текущая постфиксная строка
        private string postfixEncoded;//постфиксная строка закодированная
        private int postfixPointer;//постфиксный указатель
        private Dictionary<string, double> varValuesDictionary;//словарь значений переменных
        private Dictionary<string, UnaryFunc> functionsDictionary =
            new Dictionary<string, UnaryFunc>()
            {
                {"sin", x => Math.Round(Math.Sin(x),9)},
                {"cos", x => Math.Round(Math.Cos(x),9)},
                {"ln", x => {
                    if (x <= 0)
                    {
                        throw new Exception("Ошибка аргумента: аргумент логирифма не может быть отрицательным");
                    }
                    return Math.Log10(x);
                    }
                },
                {"tg", x =>{
                    if(Math.Round(Math.Cos(x),9) == 0){
                        throw new Exception("Ошибка аргумента: в тангенсе не может быть аргумента, который даёт косинус равный нулю");
                    }
                    return Math.Tan(x);
                }
                }
            };
        private Dictionary<string, BinaryFunc> defaultFunctionsDictionary =
            new Dictionary<string, BinaryFunc>()
            {
                {"*", (x,y) => y * x},
                {"+", (x,y) => y + x},
                {"-", (x,y) => y - x},
                {"/", (x,y) => {
                    if(x == 0){
                        throw new Exception("Ошибка аргумента: нельзя делить на 0");
                    }
                    return y / x;
                }},
                {"^", (x,y) => {
                    if(y <= 0){
                        throw new Exception("Ошибка аргумента: у степенной функции не может быть отрицательного или нулевого основания ");
                    }
                    return Math.Pow(y,x); 
                }}
            };
        

        private string defaultOperators = "-+*/^";
        public const string PUSH_VAR = "PUSH_VAR";
        public const string PUSH_CALC_UNARY = "PUSH_CALC_UNARY";
        public const string PUSH_CALC_BINARY = "PUSH_CALC_BINARY";


        public PostfixCalculator()
        {
            stack = new Stack<double>();
        }


        //задаёт новое выражение и новые переменные 
        public void changePostfixExpression(string newPostfix, Dictionary<string, double> varValues)
        {
            varValuesDictionary = varValues;
            postfix = newPostfix;
            postfixEncoded = Translator.encodeFunctions(postfix);
            postfixPointer = 0;
            stack.Clear();
            validateVariables();
        }




        //Метод который проверяет имеется ли для всех переменных значения
        private void validateVariables()
        {
            HashSet<string> variables = new HashSet<string>();
            for (int i = 0; i < postfixEncoded.Length; i++)
            {
                if (Regex.IsMatch(postfixEncoded[i].ToString(), "[a-zA-Z]"))
                {
                    if (!varValuesDictionary.ContainsKey(postfixEncoded[i].ToString()))
                    {
                        variables.Add(postfixEncoded[i].ToString());
                    }
                }
            }
            if (variables.Count != 0)
            {
                StringBuilder message = new StringBuilder("Invalid input exception: for vars: ");
                foreach (string variable in variables)
                {
                    message.Append(variable).Append(" ");
                }
                message.Append("has not found values!");
                throw new Exception(message.ToString());
            }
        }


        //Метод который моделирует вычисление и возвращает результат вычислений
        public double getPostfixValue()
        {
            while (hasNext()) {
                doOperation(getNext());
            }
            return stack.Pop();
        }

        //Метод выполняющий операцию над текущим символом
        public string doOperation(string current)
        {
            if (Regex.IsMatch(current, "[a-zA-Z]"))
            {
                stack.Push(varValuesDictionary[current]);
                postfixPointer++;
                return PUSH_VAR;
            }
            else
            {
                double val1 = 0, val2 = 0;
                if (defaultOperators.Contains(current))
                {
                    val1 = stack.Pop();
                    val2 = stack.Pop();
                    stack.Push(defaultFunctionsDictionary[current](val1, val2));
                    postfixPointer++;
                    return PUSH_CALC_BINARY;
                }
                else
                {
                    postfixPointer++;
                    stack.Push(functionsDictionary[Translator.decodeFunctions(current)](stack.Pop()));
                    return PUSH_CALC_UNARY;
                }   
            }   
        }


        public bool hasNext()
        {
            return postfixPointer != postfixEncoded.Length;
        }

        public string getNext()
        {
            return postfixEncoded[postfixPointer].ToString();
        }

        public Stack<double> getStack()
        {
            return stack;
        }

        public int getPostfixPointer()
        {
            return postfixPointer;
        }

        public string getPostfixEncoded()
        {
            return postfixEncoded;
        }

    }
}
