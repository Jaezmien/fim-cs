using fim.twilight;

namespace fim.spike.Nodes
{
    public class StatementsNode : Node 
    {
        public List<Node> Statements = new();

        public static StatementsNode ParseStatements(AST ast, TokenType expectedEndType)
        {
            StatementsNode node = new();

            while (true)
            {
                if (ast.Check(expectedEndType)) break;
                if (ast.Check(TokenType.END_OF_FILE)) { ast.ThrowSyntaxError(ast.Peek(), "Could not find end of statement body"); }
                if (ast.Check(TokenType.NEWLINE)) { ast.Next(); continue; }

                // TODO: Statement parsing
                if( ast.Peek().Type == TokenType.VARIABLE_DECLARATION )
                {
                    var variableNode = VariableDeclarationNode.Parse(ast);
                    node.Statements.Add(variableNode);
                    continue;
                }
                if( ast.Peek().Type == TokenType.PRINT || ast.Peek().Type == TokenType.PRINT_NEWLINE )
                {
                    var printNode = PrintNode.Parse(ast);
                    node.Statements.Add(printNode);
                    continue;
                }
                if( ast.Peek().Type == TokenType.LITERAL && ast.PeekNext().Type == TokenType.VARIABLE_MODIFY )
                {
                    var modifyNode = VariableModifyNode.Parse(ast);
                    node.Statements.Add(modifyNode);
                    continue;
                }

                ast.Next();
            }

            if( node.Statements.Count > 0 )
            {
                var startNode = node.Statements.First()!;
                var endNode = node.Statements.Last()!;

                node.Start = startNode.Start;
                node.Length = endNode.Start + endNode.Length - startNode.Length;
            }

            return node;
        }
    }
}
