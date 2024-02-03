using fim.twilight;

namespace fim.spike.Nodes
{
    internal class FunctionNode : Node
    {
        public string Name = "";
        public bool IsMain = false;
        public StatementsNode? Statements = null;

        public VarType? ReturnType = null;
        public Dictionary<string, VarType>? ParametersType = null;

        public static FunctionNode Parse(AST ast)
        {
            FunctionNode node = new();

            Token? startToken = null;
            if (ast.Check(TokenType.FUNCTION_MAIN))
            {
                node.IsMain = true;
                startToken = ast.Consume(TokenType.FUNCTION_MAIN, "Expected FUNCTION_MAIN");
            }
            Token headerToken = ast.Consume(TokenType.FUNCTION_HEADER, "Expected FUNCTION_HEADER");

            Token nameToken = ast.Consume(TokenType.LITERAL, "Expected LITERAL");
            node.Name = nameToken.Value;

            while (true)
            {

                if (ast.Check(TokenType.FUNCTION_PARAMETER))
                {
                    throw new NotImplementedException();
                }
                else if (ast.Check(TokenType.FUNCTION_RETURN))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    break;
                }
            }

            ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");

            node.Statements = StatementsNode.ParseStatements(ast, TokenType.FUNCTION_FOOTER);

            ast.Consume(TokenType.FUNCTION_FOOTER, "Expected FUNCTION_FOOTER");
            string expectedName = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
            if (node.Name != expectedName) ast.ThrowSyntaxError(nameToken, $"Mismatch method name -> {node.Name} & {expectedName}");
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION.");

            node.Start = startToken != null ? startToken.Start : headerToken.Start;
            node.Length = endToken.Start + endToken.Length - node.Start;

            return node;
        }
    }
}
