using fim.celestia;
using fim.spike;
using fim.twilight;

namespace fim.tests
{
    [TestClass]
    public class IO
    {
        [TestMethod]
        public void Print()
        {
            string letter = """
                Dear Princess Celestia: Inputs!

                Today I learned how to read user inputs!
                    Did you know that Applejack's speech is the word?
                    I asked Applejack's speech: "What is your speech? ".
                    I said Applejack's speech.
                That's all about how to read user inputs.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            string fakeInput = "Apples are good!";
            StringReader sr = new(fakeInput);

            Console.SetIn(sr);
            i.MainParagraph!.Execute();
            Assert.AreEqual("What is your speech? " + fakeInput + "\n", sw.ToString());
        }

        [TestMethod]
        public void Prompt_String()
        {
            string letter = """
                Dear Princess Celestia: Inputs!

                Today I learned how to read user inputs!
                    Did you know that Applejack's speech is the word?
                    I asked Applejack's speech: "What is your speech? ".
                    I said Applejack's speech.
                That's all about how to read user inputs.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            string fakeInput = "Apples are good!";
            StringReader sr = new(fakeInput);

            Console.SetIn(sr);
            i.MainParagraph!.Execute();
            Assert.AreEqual("What is your speech? " + fakeInput + "\n", sw.ToString());
        }

        [TestMethod]
        public void Prompt_Number()
        {
            string letter = """
                Dear Princess Celestia: Inputs!

                Today I learned how to read user inputs!
                    Did you know that Spike is a number?
                    I asked Spike: "What is your number? ".
                    I said Spike.
                That's all about how to read user inputs.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            using StringWriter sw = new();
            Console.SetOut(sw);

            string fakeInput = "1";
            StringReader sr = new(fakeInput);

            Console.SetIn(sr);
            i.MainParagraph!.Execute();
            Assert.AreEqual("What is your number? " + fakeInput + "\n", sw.ToString());
        }

        [TestMethod]
        public void Prompt_Number_InvalidInput()
        {
            string letter = """
                Dear Princess Celestia: Inputs!

                Today I learned how to read user inputs!
                    Did you know that Spike is a number?
                    I asked Spike: "What is your number? ".
                    I said Spike.
                That's all about how to read user inputs.

                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            string fakeInput = "Apples!";
            StringReader sr = new(fakeInput);
            Console.SetIn(sr);
            Assert.ThrowsException<FiMException>(() => i.MainParagraph!.Execute());
        }
    }
}
