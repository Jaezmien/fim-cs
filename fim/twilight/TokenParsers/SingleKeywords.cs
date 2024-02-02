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
    }
}
