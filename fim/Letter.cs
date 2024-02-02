using fim.spike;
using fim.twilight;

namespace fim 
{
    public class Letter
    {
        public Letter(string data)
        {
            var tokens = Lexer.Parse(data);
            var ast = new AST(tokens);
        }
    }
}