using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim 
{
    internal class Utilities
    {
        public static bool IsStringNumber(string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }
        public static bool IsIndentCharacter(char c) { return c == ' ' || c == '\t'; }
        public static bool IsIndentCharacter(string c ) {  return c.Length == 1 && IsIndentCharacter(char.Parse(c)); }

        public static bool IsSameClass(Type a, Type b) => a == b;
        public static bool IsSameClassOrSubclass(Type a, Type b) => IsSameClass(a, b) || a.IsSubclassOf(b);

        public static bool IsTypeHintArray(TokenType tokenType)
        {

            return tokenType switch
            {
                TokenType.TYPE_BOOLEAN_ARRAY => true,
                TokenType.TYPE_NUMBER_ARRAY => true,
                TokenType.TYPE_STRING_ARRAY => true,
                _ => false,
            };
        }
        public static bool IsTypeArray(VarType varType)
        {

            return varType switch
            {
                VarType.STRING_ARRAY => true,
                VarType.BOOLEAN_ARRAY => true,
                VarType.NUMBER_ARRAY => true,
                _ => false,
            };
        }

        public static VarType ConvertTypeHint(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.TYPE_BOOLEAN => VarType.BOOLEAN,
                TokenType.TYPE_NUMBER => VarType.NUMBER,
                TokenType.TYPE_CHAR => VarType.CHAR,
                TokenType.TYPE_STRING => VarType.STRING,
                TokenType.TYPE_BOOLEAN_ARRAY => VarType.BOOLEAN_ARRAY,
                TokenType.TYPE_NUMBER_ARRAY => VarType.NUMBER_ARRAY,
                TokenType.TYPE_STRING_ARRAY => VarType.STRING_ARRAY,
                _ => VarType.UNKNOWN,
            };
        }
        public static VarType ConvertType(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.BOOLEAN => VarType.BOOLEAN,
                TokenType.NUMBER => VarType.NUMBER,
                TokenType.CHAR => VarType.CHAR,
                TokenType.STRING => VarType.STRING,
                _ => VarType.UNKNOWN,
            };
        }

        public static object? GetDefaultValue(VarType type)
        {
            return type switch
            {
                VarType.BOOLEAN => false,
                VarType.CHAR => '\0',
                VarType.STRING => "",
                VarType.NUMBER => 0,
                _ => null,
            };
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
                    Value = GetDefaultValue((VarType)possibleNullType)!
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
                    Type = ConvertType(token.Type)
                };
                if ( lNode.Type != VarType.UNKNOWN )
                {
                    lNode.Value = token.Value;
                    return lNode;
                }
                if( token.Type == TokenType.NULL && possibleNullType != null )
                {
                    lNode.Value = GetDefaultValue(lNode.Type)!;
                    return lNode;
                }
            }
            else
            {
                BinaryExpressionNode? node = null;
                void CheckExpression(Func<bool> predicate, Func<int> index, BinaryExpressionOperator op, BinaryExpressionType type)
                {
                    if (node != null) return;
                    if (!predicate()) return;
                    var lastIndex = index();
                    var leftNode = Utilities.CreateValueNode(tokens.GetRange(0, lastIndex));
                    var rightNode = Utilities.CreateValueNode(tokens.GetRange(lastIndex + 1, tokens.Count - lastIndex - 1));

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
                    var vNode = Utilities.CreateValueNode(tokens.Skip(1).ToList());

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
