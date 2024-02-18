namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsConstantKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value != "always") { return false; }

            dequeueAmount = 0;
            return true;
        }

        internal static bool IsAndKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value != "and") { return false; }

            dequeueAmount = 0;
            return true;
        }

        internal static bool IsOrKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value != "or") { return false; }

            dequeueAmount = 0;
            return true;
        }

        internal static bool IsOfKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value != "of") { return false; }

            dequeueAmount = 0;
            return true;
        }

        internal static bool IsStatementEndKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[][] MULTI_TOKENS =
            {
                new string[] {"That", "'", "s", " ", "what", " ", "I", " ", "did" },
            };
            foreach (string[] MULTI_TOKEN in MULTI_TOKENS)
            {
                if (FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKEN))
                {
                    dequeueAmount = MULTI_TOKEN.Length - 1;
                    return true;
                }
            }

            return false;
        }
    }
}
