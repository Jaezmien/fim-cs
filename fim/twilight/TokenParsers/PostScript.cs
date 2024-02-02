namespace fim.twilight
{
    internal partial class TokenParsers 
    {
        internal static bool IsPostScriptSequence(Token currentToken, Queue<Token> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;

            if(currentToken.Value != "P" ) { return false; }
            if (oldTokens.Peek().Value != ".") { return false; };

            int postScriptSequence = 1; // 1 for S, -1 for a punctuation.
            int postScriptAmount = 1;
            for(; postScriptAmount < oldTokens.Count; postScriptAmount++)
            {
                if( postScriptAmount >= 3 && postScriptSequence == 1 && oldTokens.ElementAt(postScriptAmount).Type == TokenType.WHITESPACE ) { break; }

                if( postScriptSequence == 1 && (oldTokens.ElementAt(postScriptAmount).Value != "S") ) { return false;  }
                if( postScriptSequence != 1 && (oldTokens.ElementAt(postScriptAmount).Value != ".") ) { return false;  }

                if (oldTokens.ElementAt(postScriptAmount).Type == TokenType.NEWLINE || oldTokens.ElementAt(postScriptAmount).Type == TokenType.END_OF_FILE) { return false; } 

                postScriptSequence *= -1;
            }

            dequeueAmount = postScriptAmount;
            return true;
        }
    }
}
