using ModLR1;

namespace PostfixCalculatorUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        Translator translator = new Translator();
        PostfixCalculator postfixCalculator = new PostfixCalculator();

        [TestMethod]
        public void CalculationTest1()
        {
            /*a = 2, b = 3, c = 4, d= 5
             2 + 3 - (4 * 5) = - 15;
            */
            translator.changeInfixExpression("a+b-c*d");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",2},
                    {"b",3},
                    {"c",4},
                    {"d",5}
                });
            double val = postfixCalculator.getPostfixValue();
            Assert.AreEqual(-15, val,0.001);
        }

        [TestMethod]
        public void CalculationTest2()
        {
            translator.changeInfixExpression("a^b");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",2},
                    {"b",3},
                });
            double val = postfixCalculator.getPostfixValue();
            Assert.AreEqual(8, val, 0.001);
        }

        [TestMethod]
        public void CalculationTest3()
        {
            translator.changeInfixExpression("sin(a)");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",Math.PI},
                });
            double val = postfixCalculator.getPostfixValue();
            Assert.AreEqual(0, val, 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CalculationTest4()
        {
            translator.changeInfixExpression("a^b");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",-1},
                    {"b",Math.PI},
                });
            postfixCalculator.getPostfixValue();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CalculationTest5()
        {
            translator.changeInfixExpression("tg(a)");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",Math.PI/2},
                });
            postfixCalculator.getPostfixValue();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CalculationTest6()
        {
            translator.changeInfixExpression("ln(a)");
            postfixCalculator.changePostfixExpression(translator.translateInfix()
                , new Dictionary<string, double>()
                {
                    {"a",-3},
                });
            postfixCalculator.getPostfixValue();
        }

    }
}