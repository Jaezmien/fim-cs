namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool CanMergeReportHeader(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "Dear", " ", "Princess", " ", "Celestia", ":" };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }        
        internal static bool CanMergeReportFooter(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] EXPECTED_TOKENS = { "Your", " ", "faithful", " ", "student", "," };
            if (!FullTokenLexer.CheckTokenSequence(currentToken, oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }

    }
}
