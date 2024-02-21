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
            translator.changeInfixExpression("a+b");
            Assert.AreEqual("ab+", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest2()
        {
            translator.changeInfixExpression("a-b");
            Assert.AreEqual("ab-", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest3()
        {
            translator.changeInfixExpression("a*b");
            Assert.AreEqual("ab*", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest4()
        {
            translator.changeInfixExpression("a/b");
            Assert.AreEqual("ab/", translator.translateInfix());
        }


        [TestMethod]
        public void InfixTest5()
        {
            translator.changeInfixExpression("sin((a+b)+c)");
            Assert.AreEqual("ab+c+sin", translator.translateInfix());
        }

        [TestMethod]
        public void InfixTest6()
        {
            translator.changeInfixExpression("(a-b*c^(k/d))*(c-b)");
            Assert.AreEqual("abckd/^*-cb-*", translator.translateInfix());
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "")]
        public void InfixTest7_SyntaxValidationException()
        {
            translator.changeInfixExpression("a+*d");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "")]
        public void InfixTest8_SyntaxValidationException()
        {
            translator.changeInfixExpression("ad");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest9_SyntaxValidationException()
        {
            translator.changeInfixExpression("a(");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest10_SyntaxValidationException()
        {
            translator.changeInfixExpression(")(");
            translator.translateInfix();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxValidationException),
            "Oh my god, we can't divison on zero")]
        public void InfixTest11_SyntaxValidationException()
        {
            translator.changeInfixExpression("sincos");
            translator.translateInfix();
        }

        [TestMethod]
        public void InfixTest12()
        {
            translator.changeInfixExpression("a*sin(b)");
            Assert.AreEqual("absin*", translator.translateInfix());
        }

    }
}