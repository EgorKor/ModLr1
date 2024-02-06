using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ModLR1
{
    public class Stack
    {
        private const int DEFAULT_CAPACITY = 10;
        private int capacity;
        private List<string> data;
        private int stackPointer;

        public Stack()
        {
            capacity = DEFAULT_CAPACITY;
            data = new List<string>(capacity);
            stackPointer = capacity;
            initData();
        }

        public Stack(int capacity)
        {
            if(capacity < 0)
            {
                throw new Exception("Illegal argument exception: Capacity should be greater than zero!");
            }
            this.capacity = capacity;
            data = new List<string>(this.capacity);
            stackPointer = capacity;
            initData();
        }

        private void initData()
        {
            for(int i = 0; i < capacity; i++)
            {
                data.Add("");
            }
        }
        
        public void Push(string s)
        {
            if(stackPointer == 0)
            {
                throw new Exception("Illegal action exception: Stack is overflow!");
            }
            stackPointer--;
            data[stackPointer] = s;
        }


        //Достаёт верхний элемент стэка и перемещает указатель стэка вниз
        public string Pop()
        {
            if(stackPointer == capacity)
            {
                throw new Exception("Illegal action exception: Stack is empty!");
            }
            string popedString = data[stackPointer];
            stackPointer++;
            return popedString;
        }

        //Очищает стэк - перемещает указатель стэка на дно
        public void Clear()
        {
            stackPointer = capacity;
        }

        //Читает верхний элемент стэка, но не перемещает указатель стэка
        public string Poll()
        {
            if (stackPointer == capacity)
            {
                throw new Exception("Illegal action exception: Stack is empty!");
            }
            return data[stackPointer];
        }

        public bool isEmpty()
        {
            return stackPointer == capacity;
        }
        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < capacity - 1; i++) { 
                sb.Append(data[i] == "" ? "0x00": data[i]).Append("\n");
            }
            sb.Append(data[capacity - 1]);
            return sb.ToString();
        }
    }
}
