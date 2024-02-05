using ModLR1;
using static ModLR1.Translator;


Translator translator = new Translator("a+b");
Console.WriteLine(translator.translateInfix());
translator.changeInfixSequence("a+b+c-d");
Console.WriteLine(translator.translateInfix());
translator.changeInfixSequence("(a-b*c)*(c-b)");
Console.WriteLine(translator.translateInfix());