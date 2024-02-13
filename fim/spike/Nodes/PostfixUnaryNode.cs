using fim.twilight;

namespace fim.spike.Nodes
{
    public class PostfixUnaryNode : Node
    {
        public ValueNode? Identifier = null;
        public PostfixUnaryNodeType? Type = null;

        public static PostfixUnaryNode Parse(AST ast, bool isIncrement)
        {
            var node = new PostfixUnaryNode();

            List<Token> identifierNodes;

            if( isIncrement )
            {
                node.Type = PostfixUnaryNodeType.INCREMENT;
                identifierNodes = ast.ConsumeUntilMatch(TokenType.UNARY_INCREMENT, "Coult not find UNARY_INCREMENT");
                ast.Consume(TokenType.UNARY_INCREMENT, "Expected UNARY_INCREMENT");
            }
            else
            {
                node.Type = PostfixUnaryNodeType.DECREMENT;
                identifierNodes = ast.ConsumeUntilMatch(TokenType.UNARY_DECREMENT, "Coult not find UNARY_DECREMENT");
                ast.Consume(TokenType.UNARY_DECREMENT, "Expected UNARY_DECREMENT");
            }

            node.Identifier = AST.CreateValueNode(identifierNodes);

            var lastToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");
            node.Start = node.Identifier.Start;
            node.Length = lastToken.Start + lastToken.Length - node.Start;

            return node;
        }
    }

    public enum PostfixUnaryNodeType
    {
        UNKNOWN = 0,

        INCREMENT,
        DECREMENT
    }
}
