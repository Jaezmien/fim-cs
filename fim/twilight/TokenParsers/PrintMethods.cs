namespace fim.twilight
{
    internal partial class TokenParsers
    {
        private static readonly string[] PRINT_TOKENS = { "said", "sang", "wrote" };
        internal static bool IsPrintMethod(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if (oldTokens.Count <= 1) { return false; }

            if (currentToken.Value != "I" ||
                oldTokens.Peek().Value != " ") { return false; }

            if (Array.IndexOf(PRINT_TOKENS, oldTokens.ElementAt(1).Value) == -1) { return false; }

            dequeueAmount = 2;
            return true;
        }
        internal static bool IsPrintLnMethod(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if (oldTokens.Count <= 3) { return false; }

            if (currentToken.Value != "I" ||
                oldTokens.Peek().Value != " " ||
                oldTokens.ElementAt(1).Value != "quickly" ||
                oldTokens.ElementAt(2).Value != " ") { return false; }

            if (Array.IndexOf(PRINT_TOKENS, oldTokens.ElementAt(3).Value) == -1) { return false; }

            dequeueAmount = 4;
            return true;
        }
    }
}
