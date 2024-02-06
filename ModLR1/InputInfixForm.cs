using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModLR1
{
    public partial class InputInfixForm : Form
    {
        public InputInfixForm()
        {
            InitializeComponent();
        }


        private void aVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "a";
        }

        private void bVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "b";
        }

        private void cVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "c";
        }

        private void dVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "d";
        }

        private void eVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "e";
        }

        private void fVarButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "f";
        }

        private void openParButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "(";
        }

        private void closeParButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += ")";
        }

        private void backspaceButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text = infixTextBox.Text.Substring(0, infixTextBox.Text.Length - 1);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text = "";
        }

        private void plusButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "+";
        }

        private void minusButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "-";
        }

        private void multButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "*";
        }

        private void divButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "/";
        }

        private void powButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "^";
        }

        private void cosButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "cos";
        }

        private void lnButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "ln";
        }

        private void sinButton_Click(object sender, EventArgs e)
        {
            infixTextBox.Text += "sin";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
