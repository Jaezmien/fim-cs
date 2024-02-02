namespace fim.twilight
{
    internal enum TokenType
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

        KEYWORD_OR,
        KEYWORD_AND,
        KEYWORD_CONSTANT,
    }

    internal interface IPosition
    {
        public int Start { get; set; }
        public int Length { get; set; }
    }
    internal class RawToken : IPosition
    {
        public string Value = "";
        public int Start { get; set; }
        public int Length { get; set; }
    }
    internal class Token : RawToken
    {
        public TokenType Type;
    }
}
