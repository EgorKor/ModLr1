﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
        private Stack<string> translatorStack;
        private PostfixCalculator postfixCalculator;
        private Stack<double> calcStack;
        private Point stackPointerStartLoc;
        private int dY;
        private RichTextBox[,] actionTable;
        private RichTextBox lastMarked;
        private bool infixHasError = false;
        private Dictionary<string, string> decodeDictionary;
        private Dictionary<string, string> encodeDictionary;
        private Dictionary<int, string> arifmeticOpDictionary;
        private System.Windows.Forms.Timer translatorTimer;
        private System.Windows.Forms.Timer calcTimer;
        private int traslatorTimerInterval = 1;
        private int calcTimerInterval = 1;
        private bool isInputNotEmpty = false;
        private Dictionary<string, double> varVals;
        private bool isValuesFixed = false;
        private int calcStackPointerStartY;
        private bool postfixHasError = false;

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
            postfixCalculator = new PostfixCalculator();
            calcStack = postfixCalculator.getStack();
            decodeDictionary = Translator.getFunctionDecodeDictionary();
            encodeDictionary = Translator.getFunctionEncodeDictionary();
            lastMarked = table0_0;
            dY = 20;
            translatorTimer = new System.Windows.Forms.Timer();
            translatorTimer.Interval = traslatorTimerInterval;
            translatorTimer.Tick += new EventHandler(processOneTactTick);
            calcTimer = new System.Windows.Forms.Timer();
            calcTimer.Interval = calcTimerInterval;
            calcTimer.Tick += processOneCalcTick;
            varVals = new Dictionary<string, double>();
            calcStackPointerStartY = calcStackPointerLabel.Top;
        }


        private void inputInfixButton_Click(object sender, EventArgs e)
        {
            string infixString = openInfixDialog();
            isInputNotEmpty = updateInputInfix(infixString);
        }

        //метод обновления входной инфиксной строки
        private bool updateInputInfix(string infixString)
        {
            if (infixString == "")
            {
                MessageBox.Show("Входная строка пуста! Введите не пустую строку!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            inputTextBox.Text = infixString;
            interactiveInputTextBox.Text = infixString;
            translator.changeInfixExpression(infixString);
            infixHasError = false;
            outputTextBox.Text = "";
            stackPointerLabel.Location = stackPointerStartLoc;
            hideAll();
            return true;
        }

        //выделяет элемент таблицы
        private void markTableElement(RichTextBox needToMark)
        {
            needToMark.BackColor = Color.Blue;
            needToMark.ForeColor = Color.White;
        }

        //удаляет выделение элемента таблицы
        private void unmarkTableElement(RichTextBox needToUnmark)
        {
            needToUnmark.BackColor = Color.White;
            needToUnmark.ForeColor = Color.Black;
        }

        //открывает форму ввода инфиксной строки
        public string openInfixDialog()
        {
            Form prompt = new InputInfixForm();
            InputInfixForm castedPrompt = (InputInfixForm)prompt;
            castedPrompt.infixTextBox.Text = inputTextBox.Text;
            prompt.ShowDialog();
            return castedPrompt.isCancel ? inputTextBox.Text : castedPrompt.infixTextBox.Text;
        }

        //обработчик кнопки "Авто" - запускает автоматический режим на интерфейсе
        private void autoButton_Click(object sender, EventArgs e)
        {
            updateInputInfix(inputTextBox.Text);
            if (isInputNotEmpty)
            { 
                translatorTimer.Start();
            }
        }

        //обработчик кнопки "Такт" - запускает один такт на интерфейсе
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
                {
                    MessageBox.Show("Обработка строки завершена успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    calcPostfixTextBox.Text = outputTextBox.Text;
                    getCalculatorReady();
                }
            }
        }

        //обрабатывает один тик таймера
        private void processOneTactTick(object sender, EventArgs e)
        {
            if (translator.hasNext() && !infixHasError) {
                hideAll();
                unmarkTableElement(lastMarked);
                processOneTact();
            }
            else
            {
                translatorTimer.Stop();
                if(!infixHasError)
                {
                    calcPostfixTextBox.Text = outputTextBox.Text;
                    getCalculatorReady();
                    DialogResult res = MessageBox.Show("Обработка строки завершена успешно!\nСбросить состояние транслятора?", "Успех", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if(res == DialogResult.OK)
                    {
                        updateInputInfix(inputTextBox.Text);
                    }
                }
            }
        }


        /*Обрабатывает один такт транслятора, если во время валидации символов обрабатываемых во время такта
         возникло исключение обработка не идёт дальше, предлагается сброс состояния транслятора*/
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
                        MessageBox.Show("Ошибка синтаксической валидации: нет пары для открывающей скобки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                case Translator.OP_RES_ERR_CLOSE:
                    {
                        infixHasError = true;
                        MessageBox.Show("Ошибка синтаксической валидации: нет пары для закрывающей скобки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        translatorDelPar();
                        break;
                    }
                case Translator.OP_RES_SUCCESS:
                    {
                        break;
                    }
                default:
                    {
                        if (operation == Translator.OP_POP)
                            translatorPop();
                        else
                        {
                            outputPictureBox.Show();
                            removeFirstInfixSymbol();
                        }
                        outputTextBox.Text += Translator.decodeFunctions(operationResult);
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
            stackTextBox.Text = Translator.decodeFunctions(translatorStack.ToString());
            stackPointerLabel.Top -= dY;
        }
        
        //удаляет первый символ из входной строки 
        private void removeFirstInfixSymbol()
        {
            string encoded = Translator.encodeFunctions(interactiveInputTextBox.Text);
            string cutted = encoded.Substring(1, encoded.Length - 1);
            string decoded = Translator.decodeFunctions(cutted);
            interactiveInputTextBox.Text = decoded;
        }


        private void translatorPop()
        {
            popPictureBox.Show();
            stackPointerLabel.Top += dY;
        }

        private void translatorDelPar()
        {
            stackPointerLabel.Top += dY;
            string encoded = Translator.encodeFunctions(interactiveInputTextBox.Text);
            string cutted = encoded.Substring(1, encoded.Length - 1);
            string decoded = Translator.decodeFunctions(cutted);
            interactiveInputTextBox.Text = decoded;
        }

        private void timerIntervalTrackBar_Scroll(object sender, EventArgs e)
        {
            traslatorTimerInterval = timerIntervalTrackBar.Value * 100;
            translatorTimerIntervalLabel.Text = $"{traslatorTimerInterval} милли секунд";
            translatorTimer.Interval = traslatorTimerInterval == 0 ? 1: traslatorTimerInterval;
        }

        private void calcTimerIntervalTrackbar_Scroll(object sender, EventArgs e)
        {
            calcTimerInterval = calcTimerIntervalTrackbar.Value * 100;
            calcTimerIntervalLabel.Text = $"{calcTimerInterval} милли секунд";
            calcTimer.Interval = calcTimerInterval == 0 ? 1 : calcTimerInterval;
        }

        private void calcTactButton_Click(object sender, EventArgs e)
        {
            if (!isValuesFixed)
            {
                MessageBox.Show("Для данного выражения не инициарованы значения переменных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            processOneCalcTact();
        }

        private void calcAutoButton_Click(object sender, EventArgs e)
        {
            if (!isValuesFixed)
            {
                MessageBox.Show("Для данного выражения не инициарованы значения переменных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            calcTimer.Start();
        }

        //тик таймера калькулятора
        private void processOneCalcTick(object sender, EventArgs e)
        {
            if (postfixCalculator.hasNext() && !postfixHasError)
            {
                processOneCalcTact();
            }
            else if (!postfixHasError)
            {
                calcTimer.Stop();
                MessageBox.Show($"Строка обработана, результат вычислений = {calcStack.Poll()}", "Успех");
            }
            else
            {
                calcTimer.Stop();
                MessageBox.Show("Ранее была получена ошибка вычислений, начните процесс заново!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //метод обработки одного такта калькулятора
        private void processOneCalcTact()
        {
            if (postfixCalculator.hasNext() && !postfixHasError)
            {
                string opResult = null;
                try
                {
                    opResult = postfixCalculator.doOperation(postfixCalculator.getNext());
                }
                catch (Exception exception)
                {
                    calcTimer.Stop();
                    MessageBox.Show(exception.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    postfixHasError = true;
                    return;
                }
                switch (opResult)
                {
                    case PostfixCalculator.PUSH_VAR:
                        calcStackTextBox.Text = calcStack.ToString();
                        int needToRemoveIndex = varVals[calcPostfixTextBox.Text[postfixCalculator.getPostfixPointer() - 1].ToString()].ToString().Length;
                        calcPostfixInteractiveTextBox.Text = calcPostfixInteractiveTextBox.Text.Substring(needToRemoveIndex);
                        calcStackPointerLabel.Top -= dY;
                        break;
                    case PostfixCalculator.PUSH_CALC_UNARY:
                        needToRemoveIndex = decodeDictionary[postfixCalculator.getPostfixEncoded()[postfixCalculator.getPostfixPointer() - 1].ToString()].Length;
                        calcPostfixInteractiveTextBox.Text = calcPostfixInteractiveTextBox.Text.Substring(needToRemoveIndex);
                        calcStackTextBox.Text = calcStack.ToString();
                        break;
                    case PostfixCalculator.PUSH_CALC_BINARY:
                        calcPostfixInteractiveTextBox.Text = calcPostfixInteractiveTextBox.Text.Substring(1);
                        calcStackPointerLabel.Top += dY;
                        calcStackTextBox.Text = calcStack.ToString();
                        break;
                }
            }
            else if(!postfixHasError)
            {
                MessageBox.Show($"Строка обработана, результат вычислений = {calcStack.Poll()}","Успех");
            }
            else
            {
                MessageBox.Show("Ранее была получена ошибка вычислений, начните процесс заново!","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getCalculatorReady()
        {
            isValuesFixed = false;
            varsTextBox.Text = "";
            HashSet<string> vars = new HashSet<string>();
            string postfix = Translator.encodeFunctions(outputTextBox.Text);
            for(int i = 0; i < postfix.Length; i++)
            {
                if (Regex.IsMatch(postfix[i].ToString(),"[a-zA-Z]"))
                {
                    vars.Add(postfix[i].ToString());
                }
            }
            varVals.Clear();
            foreach (string var in vars)
            {
                varsTextBox.AppendText($"Переменная {var} = 0\n");
                varVals.Add(var,0);
            }
            
        }

        private void parseVariablesValues()
        {
            string parsingText = varsTextBox.Text;
            int parsingIterator = 0;
            int parsingIteratorLast = 0;
            for(int i = 0; i < varVals.Count; i++)
            {
                if (parsingText.Substring(parsingIterator).StartsWith("Переменная "))
                {
                    while (parsingText[parsingIterator] != '\n')
                    {
                        parsingIterator++;
                    }
                    string[] splitedSubString = parsingText.Substring(parsingIteratorLast, parsingIterator - parsingIteratorLast).Split();
                    varVals[splitedSubString[1]] = Double.Parse(splitedSubString[3].Replace(".",","));
                    parsingIterator++;
                    parsingIteratorLast = parsingIterator;
                }
            }
        }

        private void changeValuesInPostfixString()
        {
            string needToChange = calcPostfixTextBox.Text;
            foreach (KeyValuePair<string, double> entry in varVals)
            {
                needToChange = needToChange.Replace(entry.Key, entry.Value.ToString());
            }
            calcPostfixInteractiveTextBox.Text = needToChange;
        }

        private void fixingValuesButton_Click(object sender, EventArgs e)
        {
            if (calcPostfixTextBox.Text.Equals(""))
            {
                MessageBox.Show("Невозможно задать переменные для пустого выражения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                parseVariablesValues();
            }catch(Exception)
            {
                MessageBox.Show("Возникла ошибка во время парсинга. Возможно нарушен формат записи. Поля будут обновлены");
                getCalculatorReady();
            }
            changeValuesInPostfixString();
            isValuesFixed = true;
            postfixCalculator.changePostfixExpression(calcPostfixTextBox.Text, varVals);
            calcStackPointerLabel.Top = calcStackPointerStartY;
            postfixHasError = false;
        }

        private void refreshValuesTextBoxButton_Click(object sender, EventArgs e)
        {
            if (calcPostfixTextBox.Text.Equals(""))
            {
                MessageBox.Show("Невозможное действие для пустого выражения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            getCalculatorReady();
        }
    }
}
