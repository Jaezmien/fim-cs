using fim.twilight;

namespace fim.spike.Nodes
{
    public class PrintNode : Node
    {
        public bool NewLine = false;
        public ValueNode? Value = null;

        public static PrintNode Parse(AST ast)
        {
            PrintNode node = new();

            Token startToken = ast.Peek();
            if( startToken.Type != TokenType.PRINT && startToken.Type != TokenType.PRINT_INLINE )
            {
                ast.ThrowSyntaxError(startToken, "Expected PRINT or PRINT_INLINE");
            }
            node.NewLine = startToken.Type != TokenType.PRINT_INLINE;
            ast.Next();

            var valueTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Value = AST.CreateValueNode(valueTokens);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }
}
