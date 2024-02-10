using fim.twilight;
using System.Xml;

namespace fim.spike.Nodes
{
    public class IfStatementNode : Node
    {
        public IfConditionStatementNode? ifStatement = null;
        public IfStatementNode? elseStatement = null;
        
        public static IfStatementNode Parse(AST ast, int depth = 1)
        {
            var node = new IfStatementNode();

            var currentState = CurrentState.UNKNOWN; 
            while (true)
            {
                if (ast.Check(TokenType.IF_END_CLAUSE)) break;
                if (ast.Check(TokenType.END_OF_FILE)) { ast.ThrowSyntaxError(ast.Peek(), "Could not find end of if statement"); }
                if (ast.Check(TokenType.NEWLINE)) { ast.Next(); continue; }

                if( ast.Peek().Type == TokenType.IF_CLAUSE )
                {
                    // XXX: This handles the [if] statement
                    if (currentState != CurrentState.UNKNOWN) ast.ThrowSyntaxError(ast.Peek(), "Expected IF statement to be the first.");

                    node.ifStatement = IfConditionStatementNode.Parse(ast);
                    currentState = CurrentState.IF;
                }
                else if( ast.Peek().Type == TokenType.ELSE_CLAUSE )
                {
                    // XXX: This handles the [else if] statement.
                    // From what I've seen, JavaScript basically has if/else - no else if. Else if is just a shortcut for if else { if }
                    if( currentState != CurrentState.IF ) ast.ThrowSyntaxError(ast.Peek(), "Expected ELSE clause after IF statement");
                    ast.Consume(TokenType.ELSE_CLAUSE, "Expected ELSE_CLAUSE");
                    node.elseStatement = IfStatementNode.Parse(ast, depth + 1);
                    break;
                }
                else
                {
                    if( currentState != CurrentState.UNKNOWN || depth == 1 ) ast.ThrowSyntaxError(ast.Peek(), "Unknown token");
                    // XXX: This is the [else] statement.

                    var ifStatement = new IfConditionStatementNode();

                    ifStatement.Condition = new LiteralNode() { Type = VarType.BOOLEAN, RawValue = "yes" };
                    ifStatement.Body = StatementsNode.ParseStatements(ast, TokenType.IF_END_CLAUSE);

                    ifStatement.Start = ifStatement.Body.Start;
                    ifStatement.Length = ifStatement.Body.Length;

                    node.ifStatement = ifStatement;
                }
            }

            node.Start = node.ifStatement!.Start;
            node.Length = (node.elseStatement != null ? node.elseStatement.Start + node.elseStatement.Length : node.ifStatement!.Start + node.ifStatement!.Length) - node.Start;

            if( depth == 1 )
            {
                ast.Consume(TokenType.IF_END_CLAUSE, "Expected IF_END_CLAUSE");
                var endToken = ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");
                node.Length = endToken.Start + endToken.Length - node.Start;
            }

            return node; 
        }

        private enum CurrentState
        {
            UNKNOWN = 0,
            IF = 1,
            ELSE = 2,
        }
    }

    public class IfConditionStatementNode : Node
    {
        public ValueNode? Condition = null;
        public StatementsNode? Body = null;

        public static IfConditionStatementNode Parse(AST ast)
        {
            var node = new IfConditionStatementNode();

            var startToken = ast.Consume(TokenType.IF_CLAUSE, "Expected IF_CLAUSE");

            var conditionTokens = ast.ConsumeUntilMatch(t => (t.Type == TokenType.PUNCTUATION) || (t.Type == TokenType.KEYWORD_THEN), "Expected token for statement condition.");
            node.Condition = AST.CreateValueNode(conditionTokens, VarType.BOOLEAN);
            if (ast.Peek().Type == TokenType.KEYWORD_THEN) ast.Consume(TokenType.KEYWORD_THEN, "Expected KEYWORD_THEN");
            ast.Consume(TokenType.PUNCTUATION, "Expected PUNCTUATION");

            var statements = StatementsNode.ParseStatements(ast, TokenType.IF_END_CLAUSE, TokenType.ELSE_CLAUSE);
            node.Body = statements;

            node.Start = startToken.Start;
            node.Length = statements.Start + statements.Length - startToken.Start;

            return node;
        } 
    }
}
