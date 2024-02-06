namespace fim.twilight
{
    internal partial class TokenParsers
    {
        internal static bool IsInfixAddition(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "plus", "added" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) { return false;  }

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsPrefixAddition(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "add" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) { return false;  }

            dequeueAmount = 0;
            return true;
        }

        internal static bool IsInfixSubtraction(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "minus", "without" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) { return false;  }

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsPrefixSubtraction(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "subtract" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) != -1 ) {
                dequeueAmount = 0;
                return true;
            }

            string[] MULTI_TOKEN = { "the", " ", "difference", " ", "between" };
            if( FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKEN) )
            {
                dequeueAmount = MULTI_TOKEN.Length - 1;
                return true;
            }

            return false;
        }

        internal static bool IsInfixMultiplication(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "times" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) != -1 ) {
                dequeueAmount = 0;
                return true;
            }

            string[] MULTI_TOKEN = { "multiplied", " ", "with" };
            if( FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKEN) )
            {
                dequeueAmount = MULTI_TOKEN.Length - 1;
                return true;
            }

            return false;
        }
        internal static bool IsPrefixMultiplication(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "multiply" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) { return false;  }

            dequeueAmount = 0;
            return true;
        }
        internal static bool IsInfixDivision(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] MULTI_TOKEN = { "divided", " ", "by" };
            if( FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKEN) )
            {
                dequeueAmount = MULTI_TOKEN.Length - 1;
                return true;
            }

            return false;
        }
        internal static bool IsPrefixDivision(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "divide" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) == -1 ) { return false;  }

            dequeueAmount = 0;
            return true;
        }
    }
}
