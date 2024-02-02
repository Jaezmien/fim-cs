namespace fim.twilight
{
    internal partial class TokenParsers
    {
        internal static bool IsLessThanEqualOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( Array.IndexOf(new string[] { "had", "has", "is", "isn", "was", "wasn", "were", "weren" }, currentToken.Value) == -1 ) { return false;  }
            if( oldTokens.Count < 6 ) { return false; }

            string[][] MULTI_TOKENS =
            {
                new string[] {"had", " ", "no", " ", "more", " ", "than" },
                new string[] {"has", " ", "no", " ", "more", " ", "than" },
                new string[] {"is", " ", "no", " ", "greater", " ", "than" },
                new string[] {"is", " ", "no", " ", "more", " ", "than" },
                new string[] {"is", " ", "not", " ", "greater", " ", "than" },
                new string[] {"is", " ", "not", " ", "more", " ", "than" },
                new string[] {"isn", "'", "t", " ", "greater", " ", "than" },
                new string[] {"isn", "'", "t", " ", "more", " ", "than" },
                new string[] {"was", " ", "no", " ", "greater", " ", "than" },
                new string[] {"was", " ", "no", " ", "more", " ", "than" },
                new string[] {"was", " ", "not", " ", "greater", " ", "than" },
                new string[] {"was", " ", "not", " ", "more", " ", "than" },
                new string[] {"wasn", "'", "t", " ", "greater", " ", "than" },
                new string[] {"wasn", "'", "t", " ", "more", " ", "than" },
                new string[] {"were", " ", "no", " ", "greater", " ", "than" },
                new string[] {"were", " ", "no", " ", "more", " ", "than" },
                new string[] {"were", " ", "not", " ", "greater", " ", "than" },
                new string[] {"were", " ", "not", " ", "more", " ", "than" },
                new string[] {"weren", "'", "t", " ", "greater", " ", "than" },
                new string[] {"weren", "'", "t", " ", "more", " ", "than" }
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
        internal static bool IsGreaterThanEqualOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( Array.IndexOf(new string[] { "had", "has", "is", "isn", "was", "wasn", "were", "weren" }, currentToken.Value) == -1 ) { return false;  }
            if( oldTokens.Count < 6 ) { return false; }

            string[][] MULTI_TOKENS =
            {
                new string[] {"had", " ", "no", " ", "less", " ", "than" },
                new string[] {"has", " ", "no", " ", "less", " ", "than" },
                new string[] {"is", " ", "no", " ", "less", " ", "than" },
                new string[] {"is", " ", "not", " ", "less", " ", "than" },
                new string[] {"isn", "'", "t", " ", "less", " ", "than" },
                new string[] {"was", " ", "no", " ", "less", " ", "than" },
                new string[] {"was", " ", "not", " ", "less", " ", "than" },
                new string[] {"wasn", "'", "t", " ", "less", " ", "than" },
                new string[] {"were", " ", "no", " ", "less", " ", "than" },
                new string[] {"were", " ", "not", " ", "less", " ", "than" },
                new string[] {"weren", "'", "t", " ", "less", " ", "than" }
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
        internal static bool IsGreaterThanOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] tokensToCheck;

            if (Array.IndexOf(new string[] { "had", "has", "were", "was" }, currentToken.Value) != -1)
            {

                tokensToCheck = new string[] { " ", "more", "than" };
            }
            else if (Array.IndexOf(new string[] { "is", "was", "were" }, currentToken.Value) != -1)
            {

                tokensToCheck = new string[] { " ", "greater", "than" };
            }
            else
            {
                return false;
            }

            if (!FullTokenLexer.CheckTokenSequence(oldTokens, tokensToCheck)) { return false; }

            dequeueAmount = tokensToCheck.Length;
            return true;
        }
        internal static bool IsLessThanOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] PREFIX_TOKENS = { "had", "has", "is", "was", "were" };
            if (Array.IndexOf(PREFIX_TOKENS, currentToken) == -1) { return false; }

            string[] NEXT_TOKENS = { " ", "less", "than" };
            if (!FullTokenLexer.CheckTokenSequence(oldTokens, NEXT_TOKENS)) { return false; }

            dequeueAmount = NEXT_TOKENS.Length;
            return true;
        }

        internal static bool IsNotEqualOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if( Array.IndexOf(new string[] { "had", "has", "is", "isn", "was", "wasn", "were", "weren" }, currentToken.Value) == -1 ) { return false;  }
            if( oldTokens.Count < 3 ) { return false; }

            string[][] MULTI_TOKENS =
            {
                new string[] {"wasn","'","t", " ", "equal", " ", "to" },
                new string[] {"isn","'","t", " ", "equal", " ", "to" },
                new string[] {"weren","'","t", " ", "equal", " ", "to" },
                new string[] {"had","'","t"},
                new string[] {"has","'","t"},
                new string[] {"isn","'","t"},
                new string[] {"wasn","'","t"},
                new string[] {"weren","'","t"},
                new string[] {"had"," ","not"},
                new string[] {"has"," ","not"},
                new string[] {"is"," ","not"},
                new string[] {"was"," ","not"},
                new string[] {"were"," ","not"},
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

        internal static bool IsEqualOperator(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKENS = { "is", "was", "were", "had", "has", "has", "likes", "like" };
            if (Array.IndexOf(SINGLE_TOKENS, currentToken.Value) != -1)
            {
                dequeueAmount = 0;
                return true;
            }

            string[] MULTI_TOKEN_PREFIX = { "is", "was", "were" };
            if (Array.IndexOf(MULTI_TOKEN_PREFIX, currentToken.Value) == -1) { return false; }

            string[] EXPECTED_TOKENS = { " ", "equal", "to" };
            if (!FullTokenLexer.CheckTokenSequence(oldTokens, EXPECTED_TOKENS)) return false;

            dequeueAmount = EXPECTED_TOKENS.Length - 1;
            return true;
        }
    }
}
