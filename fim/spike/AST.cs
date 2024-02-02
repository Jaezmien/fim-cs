using fim.twilight;
using System.Text;

namespace fim.spike
{
    internal class AST
    {
        private Token[] Tokens;
        private int Current;

        public AST(Token[] Tokens)
        {
            this.Tokens = Tokens;
            this.Current = 0;
        }

        public Token Peek() { return Tokens[Current]; }
        public Token PeekNext() { return Tokens[Current + 1]; }
        public Token PeekPrevious() { return Tokens[Current - 1]; }
        public int PeekIndex() { return Current; }
        public void Next() { Current = Current + 1; }
        public void MoveTo(int index) { Current = index; }
        public bool Check(params TokenType[] tokenTypes)
        {
            var current = Peek();
            return tokenTypes.Any(t => t == current.Type);
        }
        public bool EndOfFile() { return Peek().Type == TokenType.END_OF_FILE; }
        public Token Consume(TokenType type, string error)
        {
            //if (!Check(type)) ThrowSyntaxError(Peek(), error);

            Next();
            return PeekPrevious();
        }
        public List<Token> ConsumeUntilMatch(TokenType type, string error)
        {
            List<Token> tokens = new List<Token>();

            while (true)
            {
                // if (EndOfFile()) ThrowSyntaxError(Peek(), error);
                if (Peek().Type == type) break;

                tokens.Add(Peek());
                Next();
            }

            return tokens;
        }
        public string JoinTokensAsString(List<Token> tokens)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Token t in tokens)
            {
                // TODO: Checks

                sb.Append(t.Value);
            }

            return sb.ToString();
        }
        public bool ExpectValue(Token token, string value, string error)
        {
            // if (token.Value != value) ThrowSyntaxError(token, error);
            return true;
        }
    }
}
