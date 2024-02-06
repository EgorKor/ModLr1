using System;
using System.Collections;
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
    public partial class MainForm : Form
    {

        Translator translator = new Translator();
        Stack translatorStack;
        private Point stackPointerStartLoc;
        private int dY = 20;
        private RichTextBox[,] actionTable;
        private RichTextBox lastMarked;

        public MainForm()
        {
            InitializeComponent();
            stackPointerStartLoc = new Point(stackPointerLabel.Location.X, stackPointerLabel.Location.Y);
            actionTable = new RichTextBox[,]
            {
                {table0_0, table0_1,table0_2,table0_3,table0_4,table0_5,table0_6,table0_7,table0_8,table0_9},
                {table1_0, table1_1,table1_2,table1_3,table1_4,table1_5,table1_6,table1_7,table1_8,table1_9},
                {table2_0, table2_1,table2_2,table2_3,table2_4,table2_5,table2_6,table2_7,table2_8,table2_9},
                {table3_0, table3_1,table3_2,table3_3,table3_4,table3_5,table3_6,table3_7,table3_8,table3_9},
                {table4_0, table4_1,table4_2,table4_3,table4_4,table4_5,table4_6,table4_7,table4_8,table4_9},
                {table5_0, table5_1,table5_2,table5_3,table5_4,table5_5,table5_6,table5_7,table5_8,table5_9},
                {table6_0, table6_1,table6_2,table6_3,table6_4,table6_5,table6_6,table6_7,table6_8,table6_9},
                {table7_0, table7_1,table7_2,table7_3,table7_4,table7_5,table7_6,table7_7,table7_8,table7_9},
            };
            translatorStack = translator.getStack();
            lastMarked = table0_0;
        }


        private void inputInfixButton_Click(object sender, EventArgs e)
        {
            string infixString = openInfixDialog();
            inputTextBox.Text = infixString;
            translator.changeInfixSequence(inputTextBox.Text);
            outputTextBox.Text = "";
            stackPointerLabel.Location = stackPointerStartLoc;
        }

        private void markTableElement(RichTextBox needToMark)
        {
            needToMark.BackColor = Color.Blue;
            needToMark.ForeColor = Color.White;
        }

        private void unmarkTableElement(RichTextBox needToUnmark)
        {
            needToUnmark.BackColor = Color.White;
            needToUnmark.ForeColor = Color.Black;
        }

        public string openInfixDialog()
        {
            Form prompt = new InputInfixForm();
            prompt.ShowDialog();
            return ((InputInfixForm)prompt).infixTextBox.Text;
        }

        private void autoButton_Click(object sender, EventArgs e)
        {
            try
            {
                translator.changeInfixSequence(inputTextBox.Text);
                outputTextBox.Text = translator.translateInfix();
            }
            catch (SyntaxValidationException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка обработки!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tactButton_Click(object sender, EventArgs e)
        {
            unmarkTableElement(lastMarked);
            if (translator.hasNext())
            {
                int beforeOperationCode = translator.nextInfixCode();
                int operation = translator.currentOperation();
                lastMarked = actionTable[translator.nextStackCode(), translator.nextInfixCode()];
                markTableElement(lastMarked);
                string operationResult = translator.processTranslatorOperation(operation);
                int afterOperationCode = translator.nextInfixCode();
                if (operation != Translator.OP_POP)
                {
                    try
                    {
                        translator.validateInfix(beforeOperationCode, afterOperationCode);
                    }
                    catch (SyntaxValidationException exception)
                    {
                        MessageBox.Show(exception.Message, "Ошибка обработки!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                switch (operationResult)
                {
                    case Translator.OP_RES_ERR_OPEN:
                        MessageBox.Show("Syntax error exception: there is no pair for closing parenthesis )!");
                        break;
                    case Translator.OP_RES_ERR_CLOSE:
                        MessageBox.Show("Syntax error exception: there is no pair for open parenthesis (!");
                        break;
                    case Translator.OP_RES_ERR_FUNC:
                        MessageBox.Show($"Syntax error exception: function parenthesis structure problem!\n" +
                            $"func #1 = {translator.functionDecodeDictionary[translatorStack.Poll()]} func #2 = {translator.functionDecodeDictionary[translator.currentInfixSequenceEncoded[translator.infixPointer].ToString()]}");
                        break;
                    case Translator.OP_RES_PUSH:
                        push();
                        break;
                    case Translator.OP_RES_DEL_PAR:
                        del_par();
                        break;
                    case Translator.OP_RES_SUCCESS:
                        break;
                    default:
                        {
                            if (operation == Translator.OP_POP)
                            {
                                pop();
                            }
                            outputTextBox.Text += translator.decodePostfix(operationResult);
                            break;
                        }
                }
            }
            else
            {
                MessageBox.Show("Success");
            }
        }

        private void push()
        {
            stackTextBox.Text = translator.decodePostfix(translatorStack.ToString());
            moveStackPointer(-dY);
        }

        private void pop()
        {
            moveStackPointer(dY);
        }

        private void del_par()
        {
            pop();
        }

        private void moveStackPointer(int dY)
        {
            stackPointerLabel.Location = new Point(stackPointerLabel.Location.X, stackPointerLabel.Location.Y + dY);
        }


    }
}
