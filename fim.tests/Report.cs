using fim.celestia;
using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim.tests
{
    [TestClass]
    public class Report
    {
        [TestMethod]
        public void Report_BasicReport()
        {
            string letter = """
                Dear Princess Celestia: Hello World!
                Your faithful student, Twilight Sparkle.
                """;
            var tokens = Lexer.Parse(letter);

            var report = spike.Nodes.Report.Parse(new AST(tokens, letter));
            var i = new Interpreter(report, letter);

            Assert.AreEqual("Hello World", i.ReportName);
            Assert.AreEqual("Twilight Sparkle", i.ReportAuthor);
        }

        [TestMethod]
        public void Report_MissingEnd_Throws()
        {
            string letter = """
                Dear Princess Celestia: Hello World!
                """;
            var tokens = Lexer.Parse(letter);

            Assert.ThrowsException<FiMException>(() => spike.Nodes.Report.Parse(new AST(tokens, letter)));
        }

        [TestMethod]
        public void Report_InvalidEnd_Throws()
        {
            string letter = """
                Dear Princess Celestia: Hello World!
                Your faithful dragon, Spike.
                """;
            var tokens = Lexer.Parse(letter);

            Assert.ThrowsException<FiMException>(() => spike.Nodes.Report.Parse(new AST(tokens, letter)));
        }
    }
}
