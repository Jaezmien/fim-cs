using fim.twilight;
using fim.spike;

namespace fim.spike.Nodes
{
    public class VariableDeclarationNode : Node
    {
        public string Identifier = "";
        public bool isConstant = false;
        public ValueNode? Value = null;
        public VarType Type = VarType.UNKNOWN;

        public static VariableDeclarationNode Parse(AST ast)
        {
            VariableDeclarationNode node = new();

            Token startToken = ast.Consume(TokenType.VARIABLE_DECLARATION, "Expected VARIABLE_DECLARATION");
            node.Identifier = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
            ast.Consume(TokenType.OPERATOR_EQ, "Expected OPERATOR_EQ");
            Token typeToken = ast.Peek();
            node.Type = Utilities.ConvertTypeHint(typeToken.Type);
            if( node.Type == VarType.UNKNOWN ) { ast.ThrowSyntaxError(typeToken, "Expected variable type"); }
            ast.Next();

            var valueTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Value = Utilities.CreateValueNode(valueTokens, node.Type);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }
}
