using fim.twilight;
using fim.spike;

namespace fim.spike.Nodes
{
    public class VariableModifyNode : Node
    {
        public string Identifier = "";
        public ValueNode? Value = null;

        public static VariableModifyNode Parse(AST ast)
        {
            VariableModifyNode node = new();

            Token startToken = ast.Consume(TokenType.LITERAL, "Expected LITERAL");
            node.Identifier = startToken.Value;
            ast.Consume(TokenType.VARIABLE_MODIFY, "Expected VARIABLE_MODIFY");

            Token typeToken = ast.Peek();
            VarType? possibleType = Utilities.ConvertTypeHint(typeToken.Type);
            if( possibleType != VarType.UNKNOWN ) { ast.Next(); }
            else { possibleType = null; }

            var valueTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Value = Utilities.CreateValueNode(valueTokens, possibleType);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }
}
