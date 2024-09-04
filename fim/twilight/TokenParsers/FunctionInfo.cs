namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsFunctionHeaderMain(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "Today", " ", "I", " ", "learned" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsFunctionHeader(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "I", " ", "learned" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }
        internal static bool IsFunctionFooter(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "That", "'", "s", " ", "all", " ", "about" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }
        internal static bool IsFunctionParameter(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value != "using" ) { return false;  }

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsFunctionReturn(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( currentToken.Value == "with" ) {
                dequeueAmount = 0;
                return true;
            }
            if (FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, new string[] { "to", " ", "get" }))
            {
                dequeueAmount = 2;
                return true;
            }

            return false;
        }
        internal static bool IsReturnKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( !FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, new string[] { "Then" , " ", "you", " ", "get" } ))
            {
                return false;
            }

            dequeueAmount = 4;
            return true;
        }

        internal static bool IsFunctionCallMethod(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if (currentToken.Value != "I") return false;
            if( oldTokens.Count < 3 ) { return false; }

            string[][] MULTI_TOKENS =
            {
                new string[] {"I", " ", "remembered"},
                new string[] {"I", " ", "would"},
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
