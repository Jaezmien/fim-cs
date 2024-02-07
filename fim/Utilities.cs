using fim.spike;
using fim.spike.Nodes;
using fim.twilight;

namespace fim 
{
    internal class Utilities
    {
        public static bool IsStringNumber(string value)
        {
            if (value.Length == 0) return false;

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
        public static VarType GetArrayBaseType(VarType varType)
        {

            return varType switch
            {
                VarType.STRING_ARRAY => VarType.STRING,
                VarType.BOOLEAN_ARRAY => VarType.BOOLEAN,
                VarType.NUMBER_ARRAY => VarType.NUMBER,
                _ => VarType.UNKNOWN,
            };
        }
        public static VarType GetArrayType(VarType varType)
        {

            return varType switch
            {
                VarType.STRING => VarType.STRING_ARRAY,
                VarType.BOOLEAN => VarType.BOOLEAN_ARRAY,
                VarType.NUMBER => VarType.NUMBER_ARRAY,
                _ => VarType.UNKNOWN,
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
                VarType.BOOLEAN_ARRAY => new Dictionary<int, bool>(),
                VarType.NUMBER_ARRAY => new Dictionary<int, double>(),
                VarType.STRING_ARRAY => new Dictionary<int, string>(),
                _ => null,
            };
        }
        public static string? GetDefaultValueString(VarType type)
        {
            return type switch
            {
                VarType.BOOLEAN => "false",
                VarType.CHAR => "'\0'",
                VarType.STRING => "\"\"",
                VarType.NUMBER => "0",
                _ => null,
            };
        }
    }
}
