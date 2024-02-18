namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsWhileKeyword(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[][] MULTI_TOKENS =
            {
                new string[] {"Here","'","s", " ", "what", " ", "I", " ", "did", " ", "while" },
                new string[] {"As", " ", "long", " ", "as" },
            };
            foreach (string[] MULTI_TOKEN in MULTI_TOKENS)
            {
                if (FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, MULTI_TOKEN))
                {
                    dequeueAmount = MULTI_TOKEN.Length - 1;
                    return true;
                }
            }

            /*string[] SINGLE_TOKENS = { "While" };
            if ( Array.IndexOf(SINGLE_TOKENS, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }*/

            return false;
        }
    }
}
