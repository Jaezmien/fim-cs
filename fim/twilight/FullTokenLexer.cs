namespace fim.twilight
{
    internal class FullTokenLexer
    {
        internal readonly static char[] PUNCTUATIONS = new char[] { '.', '!', '?', ':', ',' };
        internal static Queue<Token> CreateTokens(Queue<RawToken> rawTokens)
        {
            Queue<Token> tokens = new();

            while(rawTokens.Count > 0)
            {
                RawToken currentToken = rawTokens.Dequeue();
                TokenType tokenType = TokenType.LITERAL;

                void ProcessType(bool condition, TokenType resultType)
                {
                    if (tokenType != TokenType.LITERAL) { return; } 

                    if(!condition) { return; }
                    tokenType = resultType;
                }

                ProcessType(currentToken.Length == 1 && currentToken.Value.Length == 1 && Array.IndexOf(PUNCTUATIONS, char.Parse(currentToken.Value)) != -1, TokenType.PUNCTUATION);
                ProcessType(currentToken.Length == 1 && currentToken.Value == "\n", TokenType.NEWLINE);
                ProcessType(currentToken.Length == 1 && currentToken.Value == " ", TokenType.WHITESPACE);
                ProcessType(currentToken.Length > 1 && currentToken.Value.StartsWith("(") && currentToken.Value.EndsWith(")"), TokenType.COMMENT_PAREN);

                ProcessType(currentToken.Length > 1 && currentToken.Value.StartsWith("\"") && currentToken.Value.EndsWith("\""), TokenType.STRING);
                ProcessType(currentToken.Length > 1 && currentToken.Value.StartsWith("'") && currentToken.Value.EndsWith("'"), TokenType.CHAR);
                ProcessType(currentToken.Length > 1 && double.TryParse(currentToken.Value, out double _) || int.TryParse(currentToken.Value, out int _), TokenType.NUMBER);
                ProcessType(currentToken.Length > 1 && Array.IndexOf(new string[] {"yes", "true", "right", "correct", "no", "false", "wrong", "incorrect"}, currentToken.Value) != -1, TokenType.BOOLEAN);
                ProcessType(currentToken.Value == "nothing", TokenType.NULL);

                if (currentToken.Value.Length == 0 && tokenType == TokenType.LITERAL) continue;

                tokens.Enqueue(new Token()
                {
                    Start = currentToken.Start,
                    Length = currentToken.Length,
                    Value = currentToken.Value,
                    Type = tokenType, 
                });
            }

            Token lastToken = tokens.ElementAt(tokens.Count - 1);
            tokens.Enqueue(new Token()
            {
                Start = lastToken.Start,
                Length = 0,
                Value = "",
                Type = TokenType.END_OF_FILE
            });

            return tokens;
        }

        internal delegate bool ProcessResult(Token token, Queue<Token> oldTokens, out int dequeueAmount);
        internal static Queue<Token> MergeMultiTokens(Queue<Token> oldTokens)
        {
            Queue<Token> newTokens = new();

            void ProcessToken(Token currentToken, ProcessResult condition, TokenType resultType)
            {
                if (currentToken.Type != TokenType.LITERAL) { return; } 

                if( condition(currentToken, oldTokens, out int dequeueAmount) )
                {
                    MergeToToken(currentToken, oldTokens, dequeueAmount);
                    currentToken.Type = resultType;
                }
            }
    
            while(oldTokens.Count > 0 )
            {
                Token currentToken = oldTokens.Dequeue();

                ProcessToken(currentToken, TokenParsers.CanMergeReportHeader, TokenType.REPORT_HEADER);
                ProcessToken(currentToken, TokenParsers.CanMergeReportFooter, TokenType.REPORT_FOOTER);
                ProcessToken(currentToken, TokenParsers.IsFunctionHeaderMain, TokenType.FUNCTION_MAIN);
                ProcessToken(currentToken, TokenParsers.IsFunctionHeader, TokenType.FUNCTION_HEADER); 
                ProcessToken(currentToken, TokenParsers.IsFunctionFooter, TokenType.FUNCTION_FOOTER);
                ProcessToken(currentToken, TokenParsers.IsFunctionParameter, TokenType.FUNCTION_PARAMETER); 
                ProcessToken(currentToken, TokenParsers.IsFunctionReturn, TokenType.FUNCTION_RETURN);
                ProcessToken(currentToken, TokenParsers.IsPrintMethod, TokenType.PRINT);
                ProcessToken(currentToken, TokenParsers.IsPrintLnMethod, TokenType.PRINT_NEWLINE);

                ProcessToken(currentToken, TokenParsers.IsVariableDeclaration, TokenType.VARIABLE_DECLARATION);
                ProcessToken(currentToken, TokenParsers.IsVariableModifier, TokenType.VARIABLE_MODIFY);

                ProcessToken(currentToken, TokenParsers.IsBooleanType, TokenType.TYPE_BOOLEAN);
                ProcessToken(currentToken, TokenParsers.IsBooleanArrayType, TokenType.TYPE_BOOLEAN_ARRAY);
                ProcessToken(currentToken, TokenParsers.IsNumberType, TokenType.TYPE_NUMBER);
                ProcessToken(currentToken, TokenParsers.IsNumberArrayType, TokenType.TYPE_NUMBER_ARRAY);
                ProcessToken(currentToken, TokenParsers.IsStringType, TokenType.TYPE_STRING);
                ProcessToken(currentToken, TokenParsers.IsStringArrayType, TokenType.TYPE_STRING_ARRAY);
                ProcessToken(currentToken, TokenParsers.IsCharacterType, TokenType.TYPE_CHAR);

                ProcessToken(currentToken, TokenParsers.IsPostScriptSequence, TokenType.COMMENT_POSTSCRIPT);

                ProcessToken(currentToken, TokenParsers.IsLessThanEqualOperator, TokenType.OPERATOR_LTE);
                ProcessToken(currentToken, TokenParsers.IsGreaterThanEqualOperator, TokenType.OPERATOR_GTE);
                ProcessToken(currentToken, TokenParsers.IsGreaterThanOperator, TokenType.OPERATOR_GT);
                ProcessToken(currentToken, TokenParsers.IsLessThanOperator, TokenType.OPERATOR_LT);
                ProcessToken(currentToken, TokenParsers.IsNotEqualOperator, TokenType.OPERATOR_NEQ);
                ProcessToken(currentToken, TokenParsers.IsEqualOperator, TokenType.OPERATOR_EQ);

                ProcessToken(currentToken, TokenParsers.IsConstantKeyword, TokenType.KEYWORD_CONSTANT);
                ProcessToken(currentToken, TokenParsers.IsAndKeyword, TokenType.KEYWORD_AND);
                ProcessToken(currentToken, TokenParsers.IsOrKeyword, TokenType.KEYWORD_OR);

                newTokens.Enqueue(currentToken);
            }

            return newTokens;
        }

        internal static Queue<Token> MergeLiterals(Queue<Token> oldTokens)
        {
            Queue<Token> newTokens = new Queue<Token>();

            Queue<Token> literalToken = new Queue<Token>();

            void FlushLiteralToken()
            {
                if( literalToken.Count == 0 ) { return;  }

                Token newToken = literalToken.Dequeue();
                while( literalToken.Count > 0 )
                {
                    Token addToken = literalToken.Dequeue();
                    newToken.Value += addToken.Value;
                    newToken.Length += addToken.Length;
                }

                newTokens.Enqueue(newToken);
                literalToken.Clear();
            }

            while( oldTokens.Count > 0 )
            {
                Token currentToken = oldTokens.Dequeue();

                if( literalToken.Count == 0 )
                {
                    if( currentToken.Type != TokenType.LITERAL )
                    {
                        newTokens.Enqueue(currentToken);
                        continue;
                    }

                    literalToken.Enqueue(currentToken);
                }
                else
                {
                    if( currentToken.Type == TokenType.WHITESPACE && oldTokens.Count >= 1 && oldTokens.ElementAt(0).Type != TokenType.LITERAL && oldTokens.ElementAt(0).Type != TokenType.WHITESPACE )
                    {
                        FlushLiteralToken();
                        newTokens.Enqueue(currentToken);
                        continue;
                    }
                    if( currentToken.Type != TokenType.LITERAL && currentToken.Type != TokenType.WHITESPACE ) {
                        FlushLiteralToken();
                        newTokens.Enqueue(currentToken);
                        continue;
                    }

                    literalToken.Enqueue(currentToken);
                }
            }

            FlushLiteralToken();

            return newTokens;
        }
        internal static Queue<Token> CleanTokens(Queue<Token> oldTokens)
        {
            Queue<Token> newTokens = new Queue<Token>();

            while( oldTokens.Count > 0 )
            {
                Token currentToken = oldTokens.Dequeue();

                if( currentToken.Type == TokenType.WHITESPACE ) { continue;  }
                if( currentToken.Type == TokenType.COMMENT_PAREN ) { continue;  }
                if( currentToken.Type == TokenType.COMMENT_POSTSCRIPT ) {
                    while (oldTokens.Count > 0 && currentToken.Type != TokenType.NEWLINE)
                    {
                        currentToken = oldTokens.Dequeue();
                    }
                }
                if( currentToken.Type == TokenType.NEWLINE ) { continue;  }

                newTokens.Enqueue(currentToken);
            }

            return newTokens;
        }

        internal static void MergeToToken(Token currentToken, Queue<Token> oldTokens, int amount)
        {
            while (amount > 0)
            {
                Token preToken = oldTokens.Dequeue();
                amount--;

                currentToken.Value += preToken.Value;
                if (amount == 0)
                {
                    currentToken.Length = preToken.Start + preToken.Length - currentToken.Start;
                }
            }
        }

        internal static bool CheckTokenSequence(Token firstToken, Queue<Token> nextTokens, string[] expectedTokens)
        {
            if (nextTokens.Count < expectedTokens.Length - 1) return false;

            if (firstToken.Value != expectedTokens[0]) return false;
            for(int i = 0; i < expectedTokens.Length - 1; i++)
            {
                if (i >= nextTokens.Count) return false;
                if (nextTokens.ElementAt(i).Value != expectedTokens[i + 1]) return false;
            }

            return true;
        }
        internal static bool CheckTokenSequence(Queue<Token> nextTokens, string[] expectedTokens)
        {
            if (nextTokens.Count < expectedTokens.Length - 1) return false;

            for(int i = 0; i < expectedTokens.Length - 1; i++)
            {
                if (i >= nextTokens.Count) return false;
                if (nextTokens.ElementAt(i).Value != expectedTokens[i]) return false;
            }

            return true;
        }
    }
}
