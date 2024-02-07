using fim.spike;
using fim.spike.Nodes;
using System.ComponentModel.DataAnnotations;

namespace fim.celestia
{
    public class Interpreter
    {
        private readonly Report ReportNode;
        private readonly string Report;
        public Interpreter(Report reportNode, string report)
        {
            this.ReportNode = reportNode;
            this.Report = report;
            this.Variables = new VariableManager();
            this.Paragraphs = new Stack<Paragraph>();
            this.ReportName = reportNode.Name;
            this.ReportAuthor = reportNode.Author;

            foreach (Node n in ReportNode.Body)
            {
                if (Utilities.IsSameClass(n.GetType(), typeof(VariableDeclarationNode)))
                {
                    var node = (VariableDeclarationNode)n;
                    var value = EvaluateValueNode(node.Value, out VarType evaluatedType, false);

                    if( node.Type != evaluatedType ) ThrowRuntimeError(node, "Expected type " + node.Type + ", got " + evaluatedType);
                    if( Variables.Get(node.Identifier) != null ) ThrowRuntimeError(node, "Variable " + node.Identifier + " already exists.");

                    Variables.Push(new Variable(node.Identifier, value, node.Type, node.isConstant), true);
                }
                else if (Utilities.IsSameClass(n.GetType(), typeof(FunctionNode)))
                {
                    var node = (FunctionNode)n;
                    var paragraph = new Paragraph(this, node);

                    if( Paragraphs.FirstOrDefault(p => p.Name == paragraph.Name) != null ) ThrowRuntimeError(node, "Paragraph " + paragraph.Name + " already exists.");

                    Paragraphs.Push(paragraph);
                }
                else
                {
                    ThrowRuntimeError(n, "Execution of node " + n.GetType().Name + " is not implemented in report body.");
                }
            }
        }

        public readonly string ReportName;
        public readonly string ReportAuthor;
        public readonly VariableManager Variables;
        public readonly Stack<Paragraph> Paragraphs;
        public Paragraph? MainParagraph
        {
            get
            {
                return Paragraphs.FirstOrDefault(p => p.Main);
            }
        }

        internal object? EvaluateValueNode(ValueNode? node, out VarType resultType, bool local = false)
        {
            resultType = VarType.UNKNOWN;
            if (node == null) { return null; }

            if (Utilities.IsSameClass(node.GetType(), typeof(LiteralNode)))
            {
                var lNode = (LiteralNode)node;
                resultType = lNode.Type;
                return lNode.Value;
            }
            else if (Utilities.IsSameClass(node.GetType(), typeof(LiteralDictNode)))
            {
                var lNode = (LiteralDictNode)node;
                resultType = lNode.Type;
                return lNode.Value;
            }
            else if (Utilities.IsSameClass(node.GetType(), typeof(IdentifierNode)))
            {
                var iNode = (IdentifierNode)node;
                var variable = Variables.Get(iNode.Identifier, local);
                if (variable != null)
                {
                    resultType = variable.Type;
                    return variable.Value;
                }
            }
            else if (Utilities.IsSameClass(node.GetType(), typeof(IndexIdentifierNode)))
            {
                var iNode = (IndexIdentifierNode)node;
                var variable = Variables.Get(iNode.Identifier, local);
                if (variable != null)
                {
                    var index = EvaluateValueNode(iNode.Index, out var indexType, local);
                    if (indexType != VarType.NUMBER) throw new Exception("Expected " + VarType.NUMBER + ", got " + indexType);
                    var value = (variable.Value as Dictionary<int, object>)![Convert.ToInt32(index)];
                    return EvaluateValueNode((ValueNode)value, out resultType, true);
                }
            }
            else if (Utilities.IsSameClass(node.GetType(), typeof(BinaryExpressionNode)))
            {
                var bNode = (BinaryExpressionNode)node;
                var left = EvaluateValueNode(bNode.Left, out var leftType, local);
                var right = EvaluateValueNode(bNode.Right, out var rightType, local);

                if (left == null || right == null) return null;
                if (leftType != rightType) throw new Exception("Type mismatch");
                if (Utilities.IsTypeArray(leftType)) throw new Exception("Binary expression of an array");
                if (bNode.Type == BinaryExpressionType.ARITHMETIC && leftType != VarType.NUMBER) throw new Exception("Expected type double in arithmetic expression.");
                if (bNode.Type == BinaryExpressionType.RELATIONAL && leftType != VarType.BOOLEAN) throw new Exception("Expected type boolean in relational expression.");

                if (bNode.Type == BinaryExpressionType.ARITHMETIC) resultType = VarType.NUMBER;
                if (bNode.Type == BinaryExpressionType.RELATIONAL) resultType = VarType.BOOLEAN;

                return bNode.Operator switch
                {
                    BinaryExpressionOperator.ADD => (double)left + (double)right,
                    BinaryExpressionOperator.SUB => (double)left - (double)right,
                    BinaryExpressionOperator.MUL => (double)left * (double)right,
                    BinaryExpressionOperator.DIV => (double)left / (double)right,
                    BinaryExpressionOperator.AND => (bool)left && (bool)right,
                    BinaryExpressionOperator.OR => (bool)left || (bool)right,
                    _ => throw new NotImplementedException("Unknown operator: " + bNode.Operator),
                };
            }
            else if( Utilities.IsSameClass(node.GetType(), typeof(UnaryExpressionNode)))
            {
                var uNode = (UnaryExpressionNode)node;
                var value = EvaluateValueNode(uNode.Value, out var valueType, local);

                if( value != null && uNode.Operator == UnaryExpressionOperator.NOT )
                {
                    if (valueType != VarType.BOOLEAN) throw new Exception("Invalid usage of NOT unary");
                    return !(bool)value;
                }
            }

            throw new Exception("Unknown value: " + node);
        }

        internal void EvalauateStatementsNode(StatementsNode node)
        {
            foreach (var statement in node.Statements)
            {
                if (Utilities.IsSameClass(statement.GetType(), typeof(PrintNode)))
                {
                    var pNode = (PrintNode)statement;
                    var value = EvaluateValueNode(pNode.Value, out _, true);
                    Console.Write(value);
                    if (pNode.NewLine) { Console.Write("\n"); }
                }
                if( Utilities.IsSameClass(statement.GetType(), typeof(VariableDeclarationNode)))
                {
                    var vdNode = (VariableDeclarationNode)statement;
                    Variable var = new(vdNode.Identifier, EvaluateValueNode(vdNode.Value, out _, true), vdNode.Type, vdNode.isConstant);
                    Variables.Push(var, false);
                }
                if( Utilities.IsSameClass(statement.GetType(), typeof(VariableModifyNode)))
                {
                    var vmNode = (VariableModifyNode)statement;
                    Variable? var = Variables.Get(vmNode.Identifier, true);

                    if( var == null ) ThrowRuntimeError(vmNode, "Variable " + vmNode.Identifier + " not found.");
                    if( var!.IsConstant ) ThrowRuntimeError(vmNode, "Tried to modify variable " + vmNode.Identifier + ", which is a constant.");

                    var value = EvaluateValueNode(vmNode.Value, out VarType valueType, true);

                    if( var.Type != valueType ) ThrowRuntimeError(vmNode, "Expected type " + var.Type + ", got " + valueType);

                    var.Value = value;
                }
            }
        }

        public void ThrowRuntimeError(int index, string error)
        {
            throw new FiMException($"[line: {FiMException.GetIndexPair(Report, index).Line}] {error}");
        }
        public void ThrowRuntimeError(Node node, string error)
        {
            ThrowRuntimeError(node.Start, error);
        }
    }
}
