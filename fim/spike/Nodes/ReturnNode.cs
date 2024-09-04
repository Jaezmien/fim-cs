using fim.twilight;

namespace fim.spike.Nodes
{
    public class ReturnNode : Node
    {
        public ValueNode? Value = null;

        public static ReturnNode Parse(AST ast)
        {
            ReturnNode node = new();

            Token startToken = ast.Consume(TokenType.KEYWORD_RETURN, "Expected KEYWORD_RETURN");

            Token typeToken = ast.Peek();
            VarType? possibleType = Utilities.ConvertTypeHint(typeToken.Type);
            if( possibleType != VarType.UNKNOWN ) { ast.Next(); }
            else { possibleType = null; }

            var valueTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Value = AST.CreateValueNode(valueTokens, possibleType);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }
}
