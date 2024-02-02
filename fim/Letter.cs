using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim 
{
    public class Letter
    {
        public Letter(string report)
        {
            var tokens = Lexer.Parse(report);
            var ast = new AST(tokens, report);
            var letter = Report.Parse(ast);

            Console.WriteLine(letter.Name);
            Console.WriteLine(letter.Author);
        }
    }
}