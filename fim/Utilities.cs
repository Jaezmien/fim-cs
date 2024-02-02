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

        public static VarType GetType(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.BOOLEAN => VarType.BOOLEAN,
                TokenType.NUMBER => VarType.NUMBER,
                TokenType.CHAR => VarType.CHAR,
                TokenType.STRING => VarType.STRING,
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
        public static object GetDefaultValue(VarType type)
        {
            switch(type)
            {
                case VarType.BOOLEAN: return false;
                case VarType.CHAR: return '\0';
                case VarType.STRING: return "";
                case VarType.NUMBER: return 0;
                default: return null;
            }
        }

        public static ValueNode CreateValueNode(List<Token> tokens, VarType? possibleNullType = null)
        {
            if( tokens.Count == 1 )
            {
                var token = tokens[0];

                if( token.Type == TokenType.LITERAL )
                {
                    IdentifierNode iNode = new();
                    iNode.Identifier = token.Value;
                    iNode.Start = token.Start;
                    iNode.Length = token.Length;

                    return iNode;
                }

                LiteralNode lNode = new();
                lNode.Start = token.Start;
                lNode.Length = token.Length;
                lNode.Type = GetType(token.Type);
                if( lNode.Type != VarType.UNKNOWN )
                {
                    lNode.Value = token.Value;
                    return lNode;
                }
                if( token.Type == TokenType.NULL && possibleNullType != null )
                {
                    lNode.Value = GetDefaultValue(lNode.Type);
                    return lNode;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            throw new Exception("Unable to parse value node.");
        }
    }
}
