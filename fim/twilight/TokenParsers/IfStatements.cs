namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsIfKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "If", "When" };
            if ( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) return false;

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsThenKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if (currentToken.Value != "then") return false;

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsElseKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;
            
            if( currentToken.Value == "Otherwise" )
            {
                dequeueAmount = 0;
                return true;
            }

            string[] EXPECTED_TOKENS = { "Or" , " ", "else" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }
        internal static bool IsIfEndKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;
            
            string[] EXPECTED_TOKENS = { "That", "'", "s", " ", "what", " ", "I", " ", "would", " ", "do" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }
    }
}
