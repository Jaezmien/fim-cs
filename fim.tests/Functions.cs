using fim.celestia;
using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim.tests
{
    [TestClass]
    public class Functions 
    {
        [TestMethod]
        public void Functions_Function()
        {
            string letter = """
                I learned how to create a test.
                That's all about how to create a test.
                """;
            var tokens = Lexer.Parse(letter);

            var report = FunctionNode.Parse(new AST(tokens, letter));

            Assert.AreEqual(report.Name, "how to create a test");
            Assert.IsFalse(report.IsMain);
        }

        [TestMethod]
        public void Functions_MainFunction()
        {
            string letter = """
                Today I learned how to say hello world.
                That's all about how to say hello world.
                """;
            var tokens = Lexer.Parse(letter);

            var report = FunctionNode.Parse(new AST(tokens, letter));

            Assert.AreEqual(report.Name, "how to say hello world");
            Assert.IsTrue(report.IsMain);
        }

        [TestMethod]
        public void Functions_InvalidFunctionName_Throws()
        {
            string letter = """
                I learned how to create a letter.
                That's all about how to create a letter.
                """;
            var tokens = Lexer.Parse(letter);

            Assert.ThrowsException<FiMException>(() => FunctionNode.Parse(new AST(tokens, letter)));
        }

        [TestMethod]
        public void Functions_MissingEnd_Throws()
        {
            string letter = """
                I learned how to write a method.
                """;
            var tokens = Lexer.Parse(letter);

            Assert.ThrowsException<FiMException>(() => FunctionNode.Parse(new AST(tokens, letter)));
        }
    }
}