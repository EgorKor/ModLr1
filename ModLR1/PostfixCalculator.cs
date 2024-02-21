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
                {"sin", x => Math.Round(Math.Sin(x),3)},
                {"cos", x => Math.Round(Math.Cos(x),3)},
                {"ln", x => {
                    if (x < 0)
                    {
                        throw new Exception("Ошибка аргумента: аргумент логирифма не может быть отрицательным");
                    }
                    return Math.Log10(x);
                    }
                },
                {"tg", x =>{
                    if(Math.Round(Math.Cos(x),3) == 0){
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
                {"/", (x,y) => y / x},
                {"^", (x,y) => {
                    if(y < 0){
                        throw new Exception("Ошибка аргумента: у степенной функции не может быть отрицательного основания");
                    }return Math.Pow(y,x); }
                }
            };
        

        private string defaultOperators = "-+*/^";


        public PostfixCalculator()
        {
            stack = new Stack<double>();
            new List<string>(functionsDictionary.Keys).Min(x => x);
        }


        //задаёт новое выражение и новые переменные 
        public void changePostfixExpression(string newPostfix, Dictionary<string, double> varValues)
        {
            varValuesDictionary = varValues;
            postfix = newPostfix;
            postfixEncoded = Translator.encodeFunctions(postfix);
            postfixPointer = 0;
            stack.Clear();
        }

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

        public double getPostfixValue()
        {
            while (hasNext()) {
                doOperation(getNext());
            }
            return stack.Pop();
        }

        public string doOperation(string current)
        {
            if (Regex.IsMatch(current, "[a-zA-Z]"))
            {
                stack.Push(varValuesDictionary[current]);
                return "PUSH_VAR";
            }
            else
            {
                double val1 = 0, val2 = 0;
                if (defaultOperators.Contains(current))
                {
                    val1 = stack.Pop();
                    val2 = stack.Pop();
                    stack.Push(defaultFunctionsDictionary[current](val1, val2));
                    return "PUSH_CALC_BINARY";
                }
                else
                {
                    stack.Push(functionsDictionary[Translator.decodeFunctions(current)](stack.Pop()));
                    return "PUSH_CALC_UNARY";
                }   
            }   
        }


        public bool hasNext()
        {
            return postfixPointer != postfixEncoded.Length;
        }

        public string getNext()
        {
            return postfixEncoded[postfixPointer++].ToString();
        }



    }
}
