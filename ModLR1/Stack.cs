using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ModLR1
{
    public class Stack<T>
    {
        private const int DEFAULT_CAPACITY = 10;
        private int capacity;//емкость стека
        private T[] data;//данные стека в виде списка
        private int stackPointer = -1;//указатель на последний добавленный элемент

        public Stack()
        {
            capacity = DEFAULT_CAPACITY;
            data = new T[capacity];
        }

        public Stack(int capacity)
        {
            if(capacity < 0)
            {
                throw new Exception("Illegal argument exception: Capacity should be greater than zero!");
            }
            this.capacity = capacity;
            data = new T[this.capacity];;
        }

        //затакливает элемент в стек
        public void Push(T elem)
        {
            if (stackPointer == capacity - 1)
            {
                extend();
            }
            stackPointer++;
            data[stackPointer] = elem;
        }

        private void extend()
        {
            capacity = capacity * 2;
            T[] newData = new T[capacity];
            for(int i = 0; i < capacity / 2; i++)
            {
                newData[i] = data[i];
            }
            data = newData;
        }


        //Достаёт верхний элемент стэка и перемещает указатель стэка вниз
        public T Pop()
        {
            if(stackPointer == -1)
            {
                throw new Exception("Попытка достать элемент из пустого стека!");
            }
            T popedElem = data[stackPointer];
            stackPointer--;
            return popedElem;
        }

        //Очищает стэк - перемещает указатель стэка на дно
        public void Clear()
        {
            stackPointer = -1;
        }

        //Читает верхний элемент стэка, но не перемещает указатель стэка
        public T Poll()
        {
            return data[stackPointer];
        }

        //проверка стека на пустоту
        public bool isEmpty()
        {
            return stackPointer == -1;
        }
        
        //строчное представление данных стека
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = capacity - 1; i >= 0; i++) { 
                sb.Append(data[i].ToString() == "" ? "$": data[i].ToString()).Append("\n");
            }
            sb.Append(data[capacity - 1]);
            return sb.ToString();
        }
    }
}
