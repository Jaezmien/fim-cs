namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsVariableModifier(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "becomes", "become", "became" };
            if( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) != -1)
            {
                dequeueAmount = 0;
                return true; 
            }

            string[][] MULTI_TOKENS =
            {
                new string[] {"is", " ", "now"},
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
