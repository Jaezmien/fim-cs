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
        public bool EndOfFile() { return Peek().Type == TokenType.END_OF_FILE; }
        public Token Consume(TokenType type, string error)
        {
            if (!Check(type)) ThrowSyntaxError(Peek(), error);

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
        public List<Token> ConsumeUntilMatch(TokenType type, string error)
        {
            List<Token> tokens = new();

            while (true)
            {
                if (EndOfFile()) ThrowSyntaxError(Peek(), error);
                if (Peek().Type == type) break;

                tokens.Add(Peek());
                Next();
            }

            return tokens;
        }
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
                LiteralNode node = new()
                {
                    Start = 0, 
                    Length = 0,
                    Type = (VarType)possibleNullType,
                    Value = Utilities.GetDefaultValue((VarType)possibleNullType)!
                };

                return node;
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
                    () => tokens.FindIndex(t => t.Type == TokenType.KEYWORD_AND) != -1,
                    () => tokens.FindLastIndex(t => t.Type == TokenType.KEYWORD_AND),
                    BinaryExpressionOperator.AND, BinaryExpressionType.RELATIONAL
                );
                CheckExpression(
                    () => tokens.FindIndex(t => t.Type == TokenType.KEYWORD_OR) != -1,
                    () => tokens.FindLastIndex(t => t.Type == TokenType.KEYWORD_OR),
                    BinaryExpressionOperator.OR, BinaryExpressionType.RELATIONAL
                );

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

                if( node != null ) { return node;  }

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
