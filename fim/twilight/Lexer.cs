namespace fim.twilight
{
    internal class Lexer
    {
        public static Token[] Parse(string report)
        {
            Queue<RawToken> RawTokens = RawTokenLexer.CreateRawTokens(report);
            RawTokens = RawTokenLexer.MergeRawTokens(RawTokens);

            Queue<Token> Tokens = FullTokenLexer.CreateTokens(RawTokens);
            Tokens = FullTokenLexer.MergeMultiTokens(Tokens);
            Tokens = FullTokenLexer.MergeLiterals(Tokens);
            Tokens = FullTokenLexer.CleanTokens(Tokens);

            return Tokens.ToArray<Token>();
        }
    }
}
