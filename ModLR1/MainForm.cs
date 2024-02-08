using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

        private Translator translator;
        private Stack translatorStack;
        private Point stackPointerStartLoc;
        private int dY;
        private RichTextBox[,] actionTable;
        private RichTextBox lastMarked;
        private bool infixHasError = false;
        private Dictionary<string, string> decodeDictionary;
        private Dictionary<string, string> encodeDictionary;
        private Dictionary<int, string> arifmeticOpDictionary;
        private System.Windows.Forms.Timer timer;
        private int timerInterval = 1000;
        private bool isInputNotEmpty = false;

        public MainForm()
        {   
            InitializeComponent();
            init();
        }

        private void init()
        {
            translator = new Translator();
            outputPictureBox.Image = Image.FromFile(@"resources\output.jpg");
            popPictureBox.Image = Image.FromFile(@"resources\pop.jpg");
            pushPictureBox.Image = Image.FromFile(@"resources\push.jpg");
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
            decodeDictionary = translator.getFunctionDecodeDictionary();
            encodeDictionary = translator.getFunctionEncodeDictionary();
            lastMarked = table0_0;
            dY = 20;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(processOneTactTick);
        }


        private void inputInfixButton_Click(object sender, EventArgs e)
        {
            string infixString = openInfixDialog();
            isInputNotEmpty = updateInputInfix(infixString);
        }

        private bool updateInputInfix(string infixString)
        {
            if (infixString.Trim() == "")
            {
                MessageBox.Show("Входная строка пуста! Введите не пустую строку!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            inputTextBox.Text = infixString;
            interactiveInputTextBox.Text = infixString;
            translator.changeInfixSequence(inputTextBox.Text);
            infixHasError = false;
            outputTextBox.Text = "";
            stackPointerLabel.Location = stackPointerStartLoc;
            hideAll();
            return true;
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
            InputInfixForm castedPrompt = (InputInfixForm)prompt;
            castedPrompt.infixTextBox.Text = inputTextBox.Text;
            prompt.ShowDialog();
            return castedPrompt.isCancel ? inputTextBox.Text : castedPrompt.infixTextBox.Text;
        }

        private void autoButton_Click(object sender, EventArgs e)
        {
            updateInputInfix(inputTextBox.Text);
            if (isInputNotEmpty)
            { 
                timer.Start();
            }
        }

        private void tactButton_Click(object sender, EventArgs e)
        {
            if (!isInputNotEmpty)
            {
                MessageBox.Show("Входная строка пуста! Введите не пустую строку!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            hideAll();
            unmarkTableElement(lastMarked);
            if (translator.hasNext() && !infixHasError)
            {
                processOneTact();
            }
            else
            {
                if (infixHasError)
                {
                    DialogResult res = MessageBox.Show("Невозможно продолжить обработку строки из-за возникновения ошибки во время обработки. Сбросить состояние транслятора?", "Ошибка", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (res == DialogResult.OK)
                        updateInputInfix(inputTextBox.Text);
                }
                else
                    MessageBox.Show("Обработка строки завершена успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void processOneTactTick(object sender, EventArgs e)
        {
            if (translator.hasNext() && !infixHasError) {
                hideAll();
                unmarkTableElement(lastMarked);
                processOneTact();
            }
            else
            {
                timer.Stop();
                if(!infixHasError)
                {
                    DialogResult res = MessageBox.Show("Обработка строки завершена успешно!\nСбросить состояние транслятора?", "Успех", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if(res == DialogResult.OK)
                    {
                        updateInputInfix(inputTextBox.Text);
                    }
                }
            }
        }


        /*Обрабатывает один такт транслятора, если во время валидации символов обрабатываемых во время такта
         возникло обработка не идёт дальше, предлагается сброс состояния транслятора*/
        private void processOneTact()
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
                    infixHasError = true;
                    DialogResult res = MessageBox.Show(exception.Message + "\nСбросить состояние транслятора?", "Ошибка обработки!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    unmarkTableElement(lastMarked);
                    if (res == DialogResult.OK)
                        updateInputInfix(inputTextBox.Text);
                    return;
                }
            }
            switch (operationResult)
            {
                case Translator.OP_RES_ERR_OPEN:
                    {
                        infixHasError = true;
                        MessageBox.Show("Ошибка синтаксической валидации: нет пары для закрывающей скобки  )!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                case Translator.OP_RES_ERR_CLOSE:
                    {
                        infixHasError = true;
                        MessageBox.Show("Ошибка синтаксической валидации: нет пары для открывающей скобки (!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                case Translator.OP_RES_ERR_FUNC:
                    {
                        infixHasError = true;
                        MessageBox.Show($"Ошибка синтаксической валидации: ошибка в структуре скобок функции!\n" +
                            $"функция #1 = {decodeDictionary[translatorStack.Poll()]} функция #2 = {decodeDictionary[translator.getCurrentInfixSequenceEncoded()[translator.getInfixPointer()].ToString()]}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                case Translator.OP_RES_PUSH:
                    {
                        push();
                        break;
                    }
                case Translator.OP_RES_DEL_PAR:
                    {
                        delPar();
                        break;
                    }
                case Translator.OP_RES_SUCCESS:
                    {
                        break;
                    }
                default:
                    {
                        if (operation == Translator.OP_POP)
                            pop();
                        else
                        {
                            outputPictureBox.Show();
                            removeFirstInfixSymbol();
                        }
                        outputTextBox.Text += translator.decodeFunctions(operationResult);
                        break;
                    }
            }
        }
        private void hideAll()
        {
            popPictureBox.Hide();
            pushPictureBox.Hide();
            outputPictureBox.Hide();
        }
        //заталкивает символ из входной строки в стек (визуально)
        private void push()
        {
            pushPictureBox.Show();
            removeFirstInfixSymbol();
            stackTextBox.Text = translator.decodeFunctions(translatorStack.ToString());
            moveStackPointer(-dY);
        }
        
        //удаляет первый символ из входной строки 
        private void removeFirstInfixSymbol()
        {
            string encoded = translator.encodeFunctions(interactiveInputTextBox.Text);
            string cutted = encoded.Substring(1, encoded.Length - 1);
            string decoded = translator.decodeFunctions(cutted);
            interactiveInputTextBox.Text = decoded;
        }


        private void pop()
        {
            popPictureBox.Show();
            moveStackPointer(dY);
        }

        private void delPar()
        {
            moveStackPointer(dY);
            string encoded = translator.encodeFunctions(interactiveInputTextBox.Text);
            string cutted = encoded.Substring(1, encoded.Length - 1);
            string decoded = translator.decodeFunctions(cutted);
            interactiveInputTextBox.Text = decoded;
        }

        private void moveStackPointer(int dY)
        {
            stackPointerLabel.Location = new Point(stackPointerLabel.Location.X, stackPointerLabel.Location.Y + dY);
        }

        private void timerIntervalTrackBar_Scroll(object sender, EventArgs e)
        {
            timerInterval = timerIntervalTrackBar.Value * 100;
            timerIntervalLabel.Text = $"{timerInterval} милли секунд";
            timer.Interval = timerInterval;
        }
    }
}
