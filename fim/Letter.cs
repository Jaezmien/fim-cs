using fim.celestia;
using fim.spike;
using fim.twilight;

namespace fim 
{
    public class Letter
    {
        public static Interpreter WriteLetter(string contents)
        {
            var tokens = Lexer.Parse(contents);


            var ast = spike.Nodes.Report.Parse(new AST(tokens, contents));

            return new Interpreter(ast);
        }
    }
}