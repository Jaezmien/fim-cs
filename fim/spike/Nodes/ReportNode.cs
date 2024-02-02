using fim.twilight;

namespace fim.spike.Nodes
{
    internal class Report : Node
    {
        public string Name = "";
        public string Author = "";

        public List<Node> Body = new();

        public static Report Parse(AST ast)
        {
            Report report = new();

            Token startToken = ast.Consume(TokenType.REPORT_HEADER, "Expected REPORT_HEADER");
            report.Name = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
            ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");

            while (true)
            {
                if (ast.Check(TokenType.REPORT_FOOTER)) break;
                if (ast.Check(TokenType.END_OF_FILE)) { ast.ThrowSyntaxError(ast.Peek(), "Could not find REPORT_FOOTER"); }
                if (ast.Check(TokenType.NEWLINE)) { ast.Next(); continue; }

                if (ast.Check(TokenType.VARIABLE_DECLARATION))
                {
                    VariableDeclarationNode variableNode = VariableDeclarationNode.Parse(ast);
                    report.Body.Add(variableNode);
                    continue;
                }
                else if (ast.Check(TokenType.FUNCTION_MAIN) || ast.Check(TokenType.FUNCTION_HEADER))
                {
                    FunctionNode functionNode = FunctionNode.Parse(ast);
                    report.Body.Add(functionNode);
                    continue;
                }
                else
                {
                    ast.ThrowSyntaxError(ast.Peek(), "Unexpected token -> " + ast.Peek().Value);
                }

                ast.Next();
            }

            ast.Consume(TokenType.REPORT_FOOTER, "Expected REPORT_FOOTER");
            report.Author = ast.Consume(TokenType.LITERAL, "Expected LITERAL").Value;
            Token endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");
            if( ast.Peek().Type != TokenType.END_OF_FILE )
            {
                ast.ThrowSyntaxError(ast.Peek(), "Expected END_OF_FILE");
            }

            report.Start = startToken.Start;
            report.Length = endToken.Start + endToken.Length - startToken.Start;

            return report;
        }
    }
}
