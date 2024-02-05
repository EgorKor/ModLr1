using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 Разработать среду для моделирования преобразования инфиксной (скобочной) 
формы записи алгебраических выражений в постфиксную (бесскобочную) форму 
(в обратную польскую запись) по алгоритму Дийкстры. 
 */

/*
 Требования к организации интерфейса 
Предусмотреть два режима выполнения преобразования 
– автоматический и пошаговый.  
В процессе преобразований отображать: 
– исходное выражение в инфиксной форме в неизменном виде; 
– исходное выражение в процессе удаления из него очередного символа, 
переданного в стек или в выходную строку; 
– выходную строку, задающую выражение в постфиксной форме, 
в состояниях, отображающих процесс последовательного удлинения этой строки по мере 
передачи в нее очередных символов из стека или из исходной инфиксной строки; 
– содержимое стека. */


namespace ModLR1
{
    public partial class Form1 : Form
    {

        Stack stack;
        private const int CAPACITY = 10;
        private int dY = 10;

        public Form1()
        {
            InitializeComponent();
            init();
            for(int i = 0; i < 5; i++)
            {
                push($"{i}");
            }
            outputTextBox.Text += pop();
            outputTextBox.Text += pop();
            stackTextBox.Text = stack.ToString();
        }

        private string pop()
        {
            if (dY < 0)
                dY = -dY;
            stackPointerLabel.Location = new Point(stackPointerLabel.Location.X, stackPointerLabel.Location.Y + dY);
            return stack.Pop();
        }

        private void push(string s)
        {
            stack.Push(s);
            if(dY > 0)
                dY = -dY;
            stackPointerLabel.Location = new Point(stackPointerLabel.Location.X, stackPointerLabel.Location.Y + dY);
        }

        private void init()
        {
            stack = new Stack(CAPACITY);
        }

    }
}
