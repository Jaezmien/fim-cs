namespace fim.spike.Nodes
{
    public class IdentifierNode : ValueNode
    {
        public string Identifier = "";
    }
    public class IndexIdentifierNode : IdentifierNode
    {
        public ValueNode? Index = null;
    }
}
