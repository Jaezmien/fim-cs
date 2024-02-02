namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsNumberType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "number" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value != "a" && currentToken.Value != "the" ) { return false; }
            if( Array.IndexOf(SINGLE_TOKEN, oldTokens.ElementAt(1).Value) == -1 ) { return false;  }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsNumberArrayType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "numbers" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value != "many" && currentToken.Value != "the" ) { return false; }
            if( Array.IndexOf(SINGLE_TOKEN, oldTokens.ElementAt(1).Value) == -1 ) { return false;  }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsBooleanType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "argument", "logic" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value == "an" )
            {
                string[] NEXT_VALUE = { "argument" };
                if( Array.IndexOf(NEXT_VALUE, oldTokens.ElementAt(1).Value) == -1 ) { return false; }
            }
            else if( currentToken.Value == "the" )
            {
                string[] NEXT_VALUE = { "argument", "logic" };
                if( Array.IndexOf(NEXT_VALUE, oldTokens.ElementAt(1).Value) == -1 ) { return false; }
            }
            else { return false; }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsBooleanArrayType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "arguments", "logics" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value != "many" && currentToken.Value != "the" ) { return false; }
            if( Array.IndexOf(SINGLE_TOKEN, oldTokens.ElementAt(1).Value) == -1 ) { return false;  }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsCharacterType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "character", "letter" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value != "a"  && currentToken.Value != "the" ) { return false;  }
            if( Array.IndexOf(SINGLE_TOKEN, oldTokens.ElementAt(1).Value) == -1 ) { return false;  }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsStringType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "characters", "letters", "phrase", "quote", "sentence", "word" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value == "a" )
            {
                string[] NEXT_VALUE = { "phrase", "quote", "sentence", "word" };
                if( Array.IndexOf(NEXT_VALUE, oldTokens.ElementAt(1).Value) == -1 ) { return false; }
            }
            else if( currentToken.Value == "the" )
            {
                string[] NEXT_VALUE = { "characters", "letters", "phrase", "quote", "sentence", "word" };
                if( Array.IndexOf(NEXT_VALUE, oldTokens.ElementAt(1).Value) == -1 ) { return false; }
            }
            else { return false; }

            dequeueAmount = 2;
            return true;
        }

        internal static bool IsStringArrayType(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            string[] SINGLE_TOKEN = { "phrases", "quotes", "sentences", "words" };
            if( Array.IndexOf(SINGLE_TOKEN, currentToken.Value) != -1 )
            {
                dequeueAmount = 0;
                return true;
            }

            if( oldTokens.Count <= 1 ) { return false;  }
            if (oldTokens.Peek().Type != TokenType.WHITESPACE) { return false; }

            if( currentToken.Value != "many" && currentToken.Value != "the" ) { return false; }
            if( Array.IndexOf(SINGLE_TOKEN, oldTokens.ElementAt(1).Value) == -1 ) { return false; }

            dequeueAmount = 2;
            return true;
        }
    }
}
