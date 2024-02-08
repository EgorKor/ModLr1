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
        [ExpectedException(typeof(SyntaxValidationException),
            "")]
        public void InfixTest7_SyntaxValidationException()
        {
            translator.changeInfixSequence("a+*d");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "")]
        public void InfixTest8_SyntaxValidationException()
        {
            translator.changeInfixSequence("ad");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest9_SyntaxValidationException()
        {
            translator.changeInfixSequence("a(");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest10_SyntaxValidationException()
        {
            translator.changeInfixSequence(")(");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest11_SyntaxValidationException()
        {
            translator.changeInfixSequence("sincos");
            translator.translateInfix();
        }

        [TestMethod]
        public void InfixTest12()
        {
            translator.changeInfixSequence("a*sin(b)");
            Assert.AreEqual("absin*", translator.translateInfix());
        }

    }
}