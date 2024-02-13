using fim.twilight;

namespace fim.spike.Nodes
{
    public class StatementsNode : Node 
    {
        public List<Node> Statements = new();

        public static StatementsNode ParseStatements(AST ast, params TokenType[] expectedEndType)
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
                if( ast.Peek().Type == TokenType.PRINT || ast.Peek().Type == TokenType.PRINT_INLINE )
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
                if( ast.Contains(TokenType.UNARY_INCREMENT, new TokenType[] { TokenType.END_OF_FILE, TokenType.PUNCTUATION }.Concat(expectedEndType).ToArray() ) )
                {
                    var unaryNode = PostfixUnaryNode.Parse(ast, true);
                    node.Statements.Add(unaryNode);
                    continue;
                }
                if( ast.Contains(TokenType.UNARY_DECREMENT, new TokenType[] { TokenType.END_OF_FILE, TokenType.PUNCTUATION }.Concat(expectedEndType).ToArray() ) )
                {
                    var unaryNode = PostfixUnaryNode.Parse(ast, false);
                    node.Statements.Add(unaryNode);
                    continue;
                }
                if( ast.Contains(TokenType.KEYWORD_OF, new TokenType[] { TokenType.END_OF_FILE, TokenType.NEWLINE }.Concat(expectedEndType).ToArray() ) &&
                    ast.Contains(TokenType.OPERATOR_EQ, new TokenType[] { TokenType.END_OF_FILE, TokenType.NEWLINE }.Concat(expectedEndType).ToArray() ) )
                {
                    var modifyNode = ArrayModifyNode.Parse(ast);
                    node.Statements.Add(modifyNode);
                    continue;
                }
                if( ast.Peek().Type == TokenType.IF_CLAUSE )
                {
                    var ifNode = IfStatementNode.Parse(ast);
                    node.Statements.Add(ifNode);
                    continue;
                }

                ast.Next();
            }

            if( node.Statements.Count > 0 )
            {
                var startNode = node.Statements.First()!;
                var endNode = node.Statements.Last()!;

                node.Start = startNode.Start;
                node.Length = endNode.Start + endNode.Length - startNode.Start;
            }

            return node;
        }
    }
}
