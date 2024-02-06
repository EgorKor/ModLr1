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

        Translator translator = new Translator();
        private int dY = 10;

        public Form1()
        {
            InitializeComponent();
        }


        private void inputInfixButton_Click(object sender, EventArgs e)
        {
            string infixString = openInfixDialog();
            inputTextBox.Text = infixString;
        }

        public string openInfixDialog()
        {
            Form prompt = new InputInfixForm();
            prompt.ShowDialog();
            return ((InputInfixForm)prompt).infixTextBox.Text;
        }
    }
}
