namespace fim.spike.Nodes
{
    public class BinaryExpressionNode : ValueNode 
    {
        public ValueNode? Left;
        public BinaryExpressionOperator Operator = BinaryExpressionOperator.UNKNOWN;
        public ValueNode? Right;
        public BinaryExpressionType Type = BinaryExpressionType.UNKNOWN;
    }

    public enum BinaryExpressionType
    {
        UNKNOWN = 0,
        ARITHMETIC,
        RELATIONAL
    }

    public enum BinaryExpressionOperator
    {
        UNKNOWN = 0,

        ADD,
        SUB,
        MUL,
        DIV,

        AND,
        OR,
        GTE,
        LTE,
        GT,
        LT,
        NEQ,
        EQ
    }
}
