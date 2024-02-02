namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsVariableDeclaration(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "Did", " ", "you", " ", "know", " ", "that" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;

        }
    }
}
