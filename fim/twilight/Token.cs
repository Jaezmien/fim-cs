namespace fim.twilight
{
    public enum TokenType
    {
        UNKNOWN = 0,

        LITERAL,
        PUNCTUATION,
        NEWLINE,
        END_OF_FILE,
        WHITESPACE,
        COMMENT_PAREN,
        COMMENT_POSTSCRIPT,

        REPORT_HEADER,
        REPORT_FOOTER,

        FUNCTION_HEADER,
        FUNCTION_MAIN,
        FUNCTION_FOOTER,

        FUNCTION_RETURN,
        FUNCTION_PARAMETER,

        PRINT,
        PRINT_NEWLINE,
        PROMPT,

        VARIABLE_DECLARATION,
        VARIABLE_MODIFY,

        STRING,
        CHAR,
        NUMBER,
        BOOLEAN,
        NULL,

        TYPE_STRING,
        TYPE_CHAR,
        TYPE_NUMBER,
        TYPE_BOOLEAN,

        TYPE_STRING_ARRAY,
        TYPE_NUMBER_ARRAY,
        TYPE_BOOLEAN_ARRAY,

        OPERATOR_EQ,
        OPERATOR_NEQ,
        OPERATOR_GT,
        OPERATOR_GTE,
        OPERATOR_LT,
        OPERATOR_LTE,

        OPERATOR_ADD_INFIX,
        OPERATOR_ADD_PREFIX,
        INCREMENT,
        OPERATOR_SUB_INFIX,
        OPERATOR_SUB_PREFIX,
        DECREMENT,
        OPERATOR_MUL_INFIX,
        OPERATOR_MUL_PREFIX,
        OPERATOR_DIV_INFIX,
        OPERATOR_DIV_PREFIX,

        KEYWORD_OR,
        KEYWORD_AND,
        KEYWORD_CONSTANT,
    }

    internal interface IPosition
    {
        public int Start { get; set; }
        public int Length { get; set; }
    }
    public class RawToken : IPosition
    {
        public string Value = "";
        public int Start { get; set; }
        public int Length { get; set; }
    }
    public class Token : RawToken
    {
        public TokenType Type;
    }
}
