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
    }
}
