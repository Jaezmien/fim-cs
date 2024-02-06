namespace fim.spike.Nodes
{
    public class UnaryExpressionNode : ValueNode 
    {
        public UnaryExpressionOperator Operator = UnaryExpressionOperator.UNKNOWN;
        public ValueNode? Value;
    }

    public enum UnaryExpressionOperator
    {
        UNKNOWN = 0,

        NOT
    }
}
