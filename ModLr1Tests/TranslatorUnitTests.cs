using ModLR1;

namespace ModLr1Tests
{
    [TestClass]
    public class TranslatorUnitTests
    {
        public Translator translator = new Translator(); 

        [TestMethod]
        public void InfixTest1()
        {
            translator.changeInfixSequence("a+b");
            Assert.AreEqual("ab+", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest2()
        {
            translator.changeInfixSequence("a-b");
            Assert.AreEqual("ab-", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest3()
        {
            translator.changeInfixSequence("a*b");
            Assert.AreEqual("ab*", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest4()
        {
            translator.changeInfixSequence("a/b");
            Assert.AreEqual("ab/", translator.translateInfix());
        }


        [TestMethod]
        public void InfixTest5()
        {
            translator.changeInfixSequence("sin((a+b)+c)");
            Assert.AreEqual("ab+c+sin", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest6()
        {
            translator.changeInfixSequence("(a-b*c^(k/d))*(c-b)");
            Assert.AreEqual("abckd/^*-cb-*", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest7()
        {
            translator.changeInfixSequence("a+");
            Assert.AreEqual("", translator.translateInfix());
        }


    }
}