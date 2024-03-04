using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModLR1
{
    public partial class InputInfixForm : Form
    {
        private List<string> inputStrElems = new List<string>(); 
        public InputInfixForm()
        {
            InitializeComponent();
        }

        public bool isCancel = true;

        private string collectInputString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(string elem in inputStrElems)
            {
                sb.Append(elem);
            }
            return sb.ToString();
        }
        private void aVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "a";
            inputStrElems.Add("a");
        }

        private void bVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "b";
            inputStrElems.Add("b");
        }

        private void cVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "c";
            inputStrElems.Add("c");
        }

        private void dVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "d";
            inputStrElems.Add("d");
        }

        private void eVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "e";
            inputStrElems.Add("e");
        }

        private void fVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "f";
            inputStrElems.Add("f");
        }

        private void openParButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "(";
            inputStrElems.Add("(");
        }

        private void closeParButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += ")";
            inputStrElems.Add(")");
        }

        private void backspaceButton_Click(object sender, EventArgs e)
        {
            if (inputStrElems.Count != 0) {
                inputStrElems.RemoveAt(inputStrElems.Count - 1);
                infixTextBox.Text = collectInputString();
            }

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text = "";
            inputStrElems.Clear();
        }

        private void plusButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "+";
            inputStrElems.Add("+");
        }

        private void minusButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "-";
            inputStrElems.Add("-");
        }

        private void multButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "*";
            inputStrElems.Add("*");
        }

        private void divButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "/";
            inputStrElems.Add("/");
        }

        private void powButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "^";
            inputStrElems.Add("^");
        }

        private void cosButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "cos";
            inputStrElems.Add("cos");
        }

        private void lnButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "ln";
            inputStrElems.Add("ln");
        }

        private void sinButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "sin";
            inputStrElems.Add("sin");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(infixTextBox.Text,".*[abcdef]+.*"))
            {
                MessageBox.Show("Отсутствует хотя бы один операнд","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            isCancel = false;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tgButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "tg";
            inputStrElems.Add("tg");
        }
    }
}
