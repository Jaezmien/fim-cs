using fim.spike.Nodes;
using fim.twilight;
using System.Text;

namespace fim.spike
{
    public class AST
    {
        private readonly Token[] Tokens;
        private readonly string Report;
        private int Current;

        public AST(Token[] Tokens, string report)
        {
            this.Tokens = Tokens;
            this.Report = report;
            this.Current = 0;
        }

        public Token Peek() { return Tokens[Current]; }
        public Token PeekNext() { return Tokens[Current + 1]; }
        public Token PeekPrevious() { return Tokens[Current - 1]; }
        public int PeekIndex() { return Current; }
        public void Next() { Current++; }
        public void MoveTo(int index) { Current = index; }
        public bool Check(params TokenType[] tokenTypes)
        {
            var current = Peek();
            return tokenTypes.Any(t => t == current.Type);
        }
        public bool Contains(TokenType tokenType, params TokenType[]? stopTokens)
        {
            for(int i = Current; i < Tokens.Length; i++)
            {
                if (stopTokens != null && stopTokens.Any(s => s == Tokens[i].Type)) break;
                if (Tokens[i].Type == tokenType) return true;
            }

            return false;
        }
        public bool EndOfFile() { return Peek().Type == TokenType.END_OF_FILE; }
        public Token Consume(TokenType type, string error) => Consume(t => t.Type == type, error);
        public Token Consume(Func<Token, bool> predicate, string error)
        {
            if (!predicate(Peek())) ThrowSyntaxError(Peek(), error);

            try
            {
                PeekNext();
            }
            catch(IndexOutOfRangeException)
            {
                ThrowSyntaxError(Peek(), "Reached END_OF_FILE");
            }

            Next();
            return PeekPrevious();
        }
        public List<Token> ConsumeUntilMatch(TokenType type, string error) => ConsumeUntilMatch(t => t.Type == type, error);
        public List<Token> ConsumeUntilMatch(Func<Token, bool> predicate, string error)
        {
            List<Token> tokens = new();

            while (true)
            {
                if (EndOfFile()) ThrowSyntaxError(Peek(), error);
                if (predicate(Peek())) break;

                tokens.Add(Peek());
                Next();
            }

            return tokens;

        }
        public static string JoinTokensAsString(List<Token> tokens)
        {
            StringBuilder sb = new();

            foreach (Token t in tokens)
            {
                // TODO: Checks

                sb.Append(t.Value);
            }

            return sb.ToString();
        }
        public bool ExpectValue(Token token, string value, string error)
        {
            if (token.Value != value) ThrowSyntaxError(token, error);
            return true;
        }

        public string GetReportText(int start, int length)
        {
            return this.Report.Substring(start, length);
        }

        public void ThrowSyntaxError(int index, string error)
        {
            throw new FiMException($"[line: {FiMException.GetIndexPair(Report, index).Line}] {error}");
        }
        public void ThrowSyntaxError(Token token, string error)
        {
            ThrowSyntaxError(token.Start, error);
        }

        public static ValueNode CreateValueNode(List<Token> tokens, VarType? possibleNullType = null)
        {
            if( tokens.Count == 0 && possibleNullType != null)
            {
                if (Utilities.IsTypeArray((VarType)possibleNullType))
                {
                    LiteralDictNode node = new()
                    {
                        Start = 0,
                        Length = 0,
                        Type = (VarType)possibleNullType,
                    };

                    return node;

                }
                else
                {
                    LiteralNode node = new()
                    {
                        Start = 0,
                        Length = 0,
                        Type = (VarType)possibleNullType,
                        Value = Utilities.GetDefaultValueString((VarType)possibleNullType)!
                    };

                    return node;
                }
            }
            if( tokens.Count == 1 )
            {
                var token = tokens[0];

                if( token.Type == TokenType.LITERAL )
                {
                    IdentifierNode iNode = new()
                    {
                        Identifier = token.Value,
                        Start = token.Start,
                        Length = token.Length
                    };

                    return iNode;
                }

                LiteralNode lNode = new()
                {
                    Start = token.Start,
                    Length = token.Length,
                    Type = Utilities.ConvertType(token.Type)
                };
                if ( lNode.Type != VarType.UNKNOWN )
                {
                    lNode.Value = token.Value;
                    return lNode;
                }
                if( token.Type == TokenType.NULL && possibleNullType != null )
                {
                    lNode.Value = Utilities.GetDefaultValue(lNode.Type)!;
                    return lNode;
                }
            }
            else
            {
                if (tokens.FirstOrDefault()?.Type == TokenType.UNARY_NOT)
                {
                    var firstToken = tokens.First();
                    var valueNode = CreateValueNode(tokens.GetRange(1, tokens.Count - 1));

                    return new UnaryExpressionNode()
                    {
                        Start = firstToken.Start,
                        Length = valueNode.Start + valueNode.Length - firstToken.Start,
                        Operator = UnaryExpressionOperator.NOT,
                        Value = valueNode, 
                    };
                }

                {
                    BinaryExpressionNode? node = null;
                    void CheckExpression(Func<bool> predicate, Func<int> index, BinaryExpressionOperator op, BinaryExpressionType type)
                    {
                        if (node != null) return;
                        if (!predicate()) return;
                        var lastIndex = index();
                        var leftNode = CreateValueNode(tokens.GetRange(0, lastIndex));
                        var rightNode = CreateValueNode(tokens.GetRange(lastIndex + 1, tokens.Count - lastIndex - 1));

                        node = new BinaryExpressionNode()
                        {
                            Left = leftNode,
                            Right = rightNode,
                            Operator = op,
                            Type = type,
                            Start = leftNode.Start,
                            Length = rightNode.Start + rightNode.Length - leftNode.Start,
                        };
                    }

                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_MUL_INFIX) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_MUL_INFIX),
                        BinaryExpressionOperator.MUL, BinaryExpressionType.ARITHMETIC
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_DIV_INFIX) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_DIV_INFIX),
                        BinaryExpressionOperator.DIV, BinaryExpressionType.ARITHMETIC
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_ADD_INFIX) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_ADD_INFIX),
                        BinaryExpressionOperator.ADD, BinaryExpressionType.ARITHMETIC
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_SUB_INFIX) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_SUB_INFIX),
                        BinaryExpressionOperator.SUB, BinaryExpressionType.ARITHMETIC
                    );

                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_GTE) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_GTE),
                        BinaryExpressionOperator.GTE, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_LTE) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_LTE),
                        BinaryExpressionOperator.LTE, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_GT) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_GT),
                        BinaryExpressionOperator.GT, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_LT) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_LT),
                        BinaryExpressionOperator.LT, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_NEQ) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_NEQ),
                        BinaryExpressionOperator.NEQ, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.OPERATOR_EQ) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.OPERATOR_EQ),
                        BinaryExpressionOperator.EQ, BinaryExpressionType.RELATIONAL
                    );

                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.KEYWORD_AND) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.KEYWORD_AND),
                        BinaryExpressionOperator.AND, BinaryExpressionType.RELATIONAL
                    );
                    CheckExpression(
                        () => tokens.FindIndex(t => t.Type == TokenType.KEYWORD_OR) != -1,
                        () => tokens.FindLastIndex(t => t.Type == TokenType.KEYWORD_OR),
                        BinaryExpressionOperator.OR, BinaryExpressionType.RELATIONAL
                    );

                    if (node != null) { return node; }
                }

                if( tokens.FindIndex(t => t.Type == TokenType.PUNCTUATION && t.Value == ",") != -1 )
                {
                    Dictionary<int, ValueNode> pairs = new();
                    var baseType = Utilities.GetArrayBaseType((VarType)possibleNullType!);

                    int currentIndex = 0;
                    int currentPairIndex = 1;
                    while(true)
                    {
                        int nextIndex = tokens.FindIndex(currentIndex, t => t.Type == TokenType.PUNCTUATION && t.Value == ",");
                        var currentTokens = tokens.GetRange(currentIndex, (nextIndex == -1 ? tokens.Count : nextIndex) - currentIndex);

                        pairs[currentPairIndex] = CreateValueNode(currentTokens, baseType);

                        if (nextIndex == -1) break;
                        currentIndex = nextIndex + 1;
                        currentPairIndex += 1;
                    }

                    var firstToken = tokens.First();
                    var lastToken = tokens.Last();

                    return new LiteralDictNode()
                    {
                        Start = firstToken.Start,
                        Length = lastToken.Start + lastToken.Length - firstToken.Start,
                        RawDict = pairs,
                        Type = (VarType)possibleNullType!, 
                    };
                }

                if( tokens.FindIndex(t => t.Type == TokenType.KEYWORD_OF) != -1 )
                {
                    var ofIndex = tokens.FindIndex(t => t.Type == TokenType.KEYWORD_OF);
                    var indexTokens = tokens.GetRange(0, ofIndex);
                    var identifierTokens = tokens.GetRange(ofIndex + 1, tokens.Count - ofIndex - 1);
                    if (indexTokens.Count < 1) throw new Exception("Expected identifier");
                    if (identifierTokens.Count != 1 || identifierTokens.First().Type != TokenType.LITERAL) throw new Exception("Expected identifier");

                    var startToken = tokens.First();
                    var endToken = tokens.Last();

                    return new IndexIdentifierNode()
                    {
                        Start = startToken.Start,
                        Length = endToken.Start + endToken.Length - startToken.Start,
                        Identifier = identifierTokens.First().Value,
                        Index = CreateValueNode(indexTokens),
                    };
                }

                if( tokens.FirstOrDefault() != null && Utilities.ConvertTypeHint(tokens.First().Type) != VarType.UNKNOWN )
                {
                    var expectedType = Utilities.ConvertTypeHint(tokens.First().Type);
                    var vNode = CreateValueNode(tokens.Skip(1).ToList());

                    if( Utilities.IsSameClass(vNode.GetType(), typeof(LiteralNode)))
                    {
                        var lNode = (LiteralNode)vNode;
                        if( lNode.Type != expectedType ) { throw new Exception("Expected type " + expectedType + ", got " + lNode.Type);  }
                    }
                    /*else if( Utilities.IsSameClass(vNode.GetType(), typeof(IdentifierNode)))
                    {
                        var lNode = (LiteralNode)vNode;
                        if( lNode.Type != expectedType ) { throw new Exception("Expected type " + expectedType + ", got " + lNode.Type);  }
                    }*/
                    else if( Utilities.IsSameClass(vNode.GetType(), typeof(BinaryExpressionNode)))
                    {
                        var bNode = (BinaryExpressionNode)vNode;
                        if( bNode.Type == BinaryExpressionType.ARITHMETIC && expectedType != VarType.NUMBER ) { throw new Exception("Expected type " + expectedType + ", got " + bNode.Type);  }
                        else if( bNode.Type == BinaryExpressionType.RELATIONAL && expectedType != VarType.BOOLEAN ) { throw new Exception("Expected type " + expectedType + ", got " + bNode.Type);  }
                    }

                    return vNode;
                }


                throw new NotImplementedException();
            }

            throw new Exception("Unable to parse value node.");
        }
    }
}
