using fim.celestia;
using fim.spike;
using fim.twilight;

namespace fim.tests
{
    [TestClass]
    public class Samples
    {
        [TestMethod]
        public void HelloWorld()
        {
            string letter = """
                Dear Princess Celestia: Hello World!
                Today I learned how to say hello world.
                    I said "Hello World".
                That's all about how to say hello world.
                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = fim.spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            Assert.IsNotNull(i.MainParagraph);
            i.MainParagraph.Execute();
            Assert.AreEqual("Hello World\n", sw.ToString());
        }

        [TestMethod]
        public void Arrays()
        {
            string letter = """
                Dear Princess Celestia: Arrays!
                I learned how to make arrays!
                    Did you know that cake has many words?

                    Did you know that carrot is the number 4?
                    Did you know that banana cake is the word "Banana Cake"?

                    1 of cake is the word "Mango Cake".
                    2 of cake is the word "Strawberry Cake".
                    3 of cake is banana cake.
                    carrot of cake is "Carrot Cake".

                    carrot got one less.

                    I said carrot of cake.
                That's all about how to make arrays.

                I learned how to make pre-defined arrays!
                    Did you know that Apples has the words "Gala", "Red Delicious", "Mcintosh", "Honeycrisp"?
                    I said 1 of Apples.
                That's all about how to make pre-defined arrays.
                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            var makeArray = i.Paragraphs.FirstOrDefault(p => p.Name == "how to make arrays");
            Assert.IsNotNull(makeArray);
            makeArray.Execute();
            Assert.AreEqual("Banana Cake\n", sw.ToString());

            sw.GetStringBuilder().Clear(); // Reset StringWriter

            var definedArray = i.Paragraphs.FirstOrDefault(p => p.Name == "how to make pre-defined arrays");
            Assert.IsNotNull(definedArray);
            definedArray.Execute();

        }

        [TestMethod]
        public void Conditionals()
        {
            string letter = """
                Dear Princess Celestia: Conditionals!

                Did you know that Twilight is the argument correct?
                Did you know that Spike is the argument incorrect?

                I learned how to test for equality!
                    I said Twilight is equal to Spike.
                    I said Twilight is equal to the logic yes.
                    I said Twilight is equal to the logic no.
                    I said Spike is equal to the logic true.
                    I said Spike is equal to the logic false.
                That's all about how to test for equality.

                I learned how to test for inequality!
                    I said Twilight isn't equal to Spike.
                    I said Twilight isn't equal to the logic yes.
                    I said Twilight isn't equal to the logic no.
                    I said Spike isn't equal to the logic true.
                    I said Spike isn't equal to the logic false.
                That's all about how to test for inequality.

                I learned how to test operators!
                    I said Twilight and Spike.
                    I said Twilight or Spike.
                    I said not Twilight.
                    I said not Spike.
                That's all about how to test operators.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            var eqTest = i.Paragraphs.FirstOrDefault(p => p.Name == "how to test for equality");
            Assert.IsNotNull(eqTest);
            eqTest.Execute();
            Assert.AreEqual(string.Join("", new bool[] { false, true, false, false, true }.Select(b => b.ToString() + "\n")), sw.ToString());

            sw.GetStringBuilder().Clear();

            var neqTest = i.Paragraphs.FirstOrDefault(p => p.Name == "how to test for inequality");
            Assert.IsNotNull(neqTest);
            neqTest.Execute();
            Assert.AreEqual(string.Join("", new bool[] { true, false, true, true, false }.Select(b => b.ToString() + "\n")), sw.ToString());

            sw.GetStringBuilder().Clear();

            var opTest = i.Paragraphs.FirstOrDefault(p => p.Name == "how to test operators");
            Assert.IsNotNull(opTest);
            opTest.Execute();
            Assert.AreEqual(string.Join("", new bool[] { false, true, false, true }.Select(b => b.ToString() + "\n")), sw.ToString());
        }

        [Ignore]
        [TestMethod]
        public void ForLoops()
        {
            string letter = """
                Dear Princess Celestia: For Loops!

                I learned how to do a numerical for loop!
                    For every number index from 0 to 100...
                        I said index.
                    That's what I did.
                That's all about how to do a numerical for loop.

                I learned how to iterate through a string!
                    Did you know that Applejack is the word "Apples"?

                    For each character apple in Applejack...
                        I said apple.
                    That's what I did.
                That's all about how to iterate through a string.

                I learned how to iterate through an array!
                    Did you know that Apples has the words "Gala", "Red Delicious", "Mcintosh", "Honeycrisp"?
                    
                    For every word apple in Apples...
                        I said apple.
                    That's what I did.
                That's all about how to iterate through an array.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            var forNumber = i.Paragraphs.FirstOrDefault(p => p.Name == "how to do a numerical for loop");
            Assert.IsNotNull(forNumber);
            forNumber.Execute();
            Assert.AreEqual(string.Join("", new int[100].Select((_, i) => i).Select(b => b.ToString() + "\n")), sw.ToString());

            sw.GetStringBuilder().Clear();

            var forString = i.Paragraphs.FirstOrDefault(p => p.Name == "how to iterate through a string");
            Assert.IsNotNull(forString);
            forString.Execute();
            Assert.AreEqual(string.Join("", "Apples".ToCharArray().Select(b => b.ToString() + "\n")), sw.ToString());

            sw.GetStringBuilder().Clear();

            var forArray = i.Paragraphs.FirstOrDefault(p => p.Name == "how to iterate thorugh an array");
            Assert.IsNotNull(forArray);
            forArray.Execute();
            Assert.AreEqual(string.Join("", new string[] { "Gala", "Red Delicious", "Mcintosh", "Honeycrisp" }.Select(b => b.ToString() + "\n")), sw.ToString());
        }

        [Ignore]
        [TestMethod]
        public void Input()
        {
            string letter = """
                Dear Princess Celestia: Inputs!

                Today I learned how to read user inputs!
                    Did you know that Applejack's speech is the word?
                    I asked Applejack's speech: "What is your speech?".
                    I said Applejack's speech.
                That's all about how to read user inputs.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            string fakeInput = "Apples!";
            StringReader sr = new(fakeInput);

            Console.SetIn(sr);
            i.MainParagraph!.Execute();
            Assert.AreEqual(fakeInput + "\n", sw.ToString());
        }

        [Ignore]
        [TestMethod]
        public void FunctionParams()
        {
            string letter = """
                Dear Princess Celestia: Functions!
                
                I learned how to take multiple parameters using the word x and the number y.
                    I said x.
                    I said y.
                That's all about how to take multiple parameters.

                Today I learned how to run a function!
                    I remembered how to take multiple parameters using "x" and 1.
                    I remembered how to take multiple parameters using "y".
                That's all about how to run a function.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            i.MainParagraph!.Execute();
            Assert.AreEqual(string.Join("", new string[] { "x", "1", "y", "0" }.Select(b => b.ToString() + "\n")), sw.ToString());
        }

        [TestMethod]
        public void StringIndex()
        {
            string letter = """
                Dear Princess Celestia: Indexes!
                
                Today I learned how to take an index!
                    Did you know that Spike is the word "Dragon"?
                    Did you know that Owlowiscious is the number 1?

                    I quickly said Owlowiscious of Spike.
                    Owlowiscious got one more.
                    I quickly said Owlowiscious of Spike.
                That's all about how to take an index.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            i.MainParagraph!.Execute();
            Assert.AreEqual("Dr", sw.ToString());
        }

        [TestMethod]
        public void Unary()
        {
            string letter = """
                Dear Princess Celestia: Unaries!
                
                I learned how to execute variable unaries!
                    Did you know that Spike is the number 1?
                    I said Spike.
                    Spike got one more.
                    I said Spike.
                    Spike got one less.
                    I said Spike.
                That's all about how to execute variable unaries.

                I learned how to execute array unaries!
                    Did you know that Owlowiscious is the numbers 1, 2, 3?
                    I quickly said 1 of Owlowiscious.
                    I quickly said 2 of Owlowiscious.
                    I quickly said 3 of Owlowiscious.
                    I quickly said "\n".

                    2 of Owlowiscious got one more.

                    I quickly said 1 of Owlowiscious.
                    I quickly said 2 of Owlowiscious.
                    I quickly said 3 of Owlowiscious.
                That's all about how to execute array unaries.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            var varUnary = i.Paragraphs.FirstOrDefault(p => p.Name == "how to execute variable unaries");
            Assert.IsNotNull(varUnary);
            varUnary.Execute();
            Assert.AreEqual(string.Join("", new string[] { "1", "2", "1" }.Select(b => b.ToString() + "\n")), sw.ToString());

            sw.GetStringBuilder().Clear();

            var arrUnary = i.Paragraphs.FirstOrDefault(p => p.Name == "how to execute array unaries");
            Assert.IsNotNull(arrUnary);
            arrUnary.Execute();
            Assert.AreEqual("123\n133", sw.ToString());
        }

        [Ignore]
        [TestMethod]
        public void WhileLoop()
        {
            string letter = """
                Dear Princess Celestia: While Loops!

                Today I learned how to run loops!
                    Did you know that Spike is the number 1?
                    
                    As long as Spike is no greater than 5...
                        I said Spike.
                        Spike got one less.
                    That's what I did.
                That's all about how to run loops.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            i.MainParagraph!.Execute();
            Assert.AreEqual(string.Join("", new string[] { "1", "2", "3", "4", "5" }.Select(b => b.ToString() + "\n")), sw.ToString());
        }
    }
}