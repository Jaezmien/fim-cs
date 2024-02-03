using fim.celestia;
using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim 
{
    public class Letter
    {
        public static Report PrepareLetter(string contents)
        {
            var tokens = Lexer.Parse(contents);
            return Report.Parse(new AST(tokens, contents));
        }
        public static Interpreter WriteLetter(string contents)
        {
            var ast = PrepareLetter(contents);
            return new Interpreter(ast);
        }
    }
}