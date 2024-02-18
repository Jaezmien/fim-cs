using fim.twilight;
using System.Xml;

namespace fim.spike.Nodes
{
    public class WhileStatementNode : Node
    {
        public ValueNode? Condition = null;
        public StatementsNode? Body = null;
        
        public static WhileStatementNode Parse(AST ast)
        {
            var node = new WhileStatementNode();

            var startToken = ast.Consume(TokenType.WHILE_CLAUSE, "Expected WHILE_CLAUSE");

            var conditionTokens = ast.ConsumeUntilMatch(t => (t.Type == TokenType.PUNCTUATION) || (t.Type == TokenType.KEYWORD_THEN), "Expected token for statement condition.");
            node.Condition = AST.CreateValueNode(conditionTokens, VarType.BOOLEAN);
            if (ast.Peek().Type == TokenType.KEYWORD_THEN) ast.Consume(TokenType.KEYWORD_THEN, "Expected KEYWORD_THEN");
            ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");

            var statements = StatementsNode.ParseStatements(ast, TokenType.KEYWORD_STATEMENT_END);
            node.Body = statements;

            node.Start = startToken.Start;
            node.Length = statements.Start + statements.Length - startToken.Start;

            return node;
        }
    }
}
