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

    public class PromptNode : Node
    {
        public string Identifier = "";
        public ValueNode? Prompt = null;
        
        public static PromptNode Parse(AST ast)
        {
            PromptNode node = new();

            Token startToken = ast.Consume(TokenType.PROMPT, "Expected PROMPT");
            node.Identifier = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
            ast.Consume((t) => t.Type == TokenType.PUNCTUATION && t.Value == ":", "Expected PUNCTUATION_COLON");

            var promptTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Prompt = AST.CreateValueNode(promptTokens);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }
}
