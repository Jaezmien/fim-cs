using fim.twilight;

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
            node.Value = AST.CreateValueNode(valueTokens, possibleType);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken.Start;
            node.Length = endToken.Start + endToken.Length - startToken.Start;

            return node;
        }
    }

    public class ArrayModifyNode : Node
    {
        public IndexIdentifierNode? Identifier = null;
        public ValueNode? Value = null;

        public static ArrayModifyNode Parse(AST ast)
        {
            ArrayModifyNode node = new();

            var identifierNodes = ast.ConsumeUntilMatch(TokenType.OPERATOR_EQ, "Could not find OPERATOR_EQ");
            var identifierNode = AST.CreateValueNode(identifierNodes);
            if (!Utilities.IsSameClass(identifierNode.GetType(), typeof(IndexIdentifierNode))) throw new Exception("Expected IndexedIdentifierNode");
            node.Identifier = (IndexIdentifierNode)identifierNode;
            ast.Consume(TokenType.OPERATOR_EQ, "Expected OPERATOR_EQ");

            Token typeToken = ast.Peek();
            VarType? possibleType = Utilities.ConvertTypeHint(typeToken.Type);
            if( possibleType != VarType.UNKNOWN ) { ast.Next(); }
            else { possibleType = null; }

            var valueTokens = ast.ConsumeUntilMatch(TokenType.PUNCTUATION, "Could not find PUNCTUATION");
            node.Value = AST.CreateValueNode(valueTokens, possibleType);
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = identifierNodes[0].Start;
            node.Length = endToken.Start + endToken.Length - identifierNodes[0].Start;

            return node;
        }
    }
}
