namespace fim.twilight
{
    internal partial class TokenParsers
    {
        internal static bool IsUnaryIncrement(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] MULTI_TOKENS = { "got", " ", "one", " ", "more" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKENS)) { return false; }

            dequeueAmount = MULTI_TOKENS.Length - 1;
            return true;
        }

        internal static bool IsUnaryDecrement(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] MULTI_TOKENS = { "got", " ", "one", " ", "less" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKENS)) { return false; }

            dequeueAmount = MULTI_TOKENS.Length - 1;
            return true;
        }

        internal static bool IsUnaryNot(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if (currentToken.Value == "not")
            {
                dequeueAmount = 0;
                return true;
            }

            string[] MULTI_TOKENS = { "it", "'", "s", " ", "not", " ", "the", " ", "case", " ", "that" };
            if (FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKENS))
            {
                dequeueAmount = MULTI_TOKENS.Length - 1;
                return true;
            }

            return false;
        }
    }
}
