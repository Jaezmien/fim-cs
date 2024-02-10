using fim.spike;
using fim.spike.Nodes;

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
                var nodeDict = lNode.Value as Dictionary<int, ValueNode>;
                var expectedType = Utilities.GetArrayBaseType(lNode.Type);

                if( lNode.Type == VarType.BOOLEAN_ARRAY )
                {
                    var dict = new Dictionary<int, bool>();
                    if (nodeDict != null)
                    {
                        foreach (KeyValuePair<int, ValueNode> entry in nodeDict)
                        {
                            var entryValue = EvaluateValueNode(entry.Value, out var entryType, true);
                            if (entryType != expectedType) throw new Exception("Expected " + expectedType + ", got " + entryType);
                            dict[entry.Key] = (bool)entryValue;
                        }
                    }
                    resultType = VarType.BOOLEAN_ARRAY;
                    return dict;
                }
                if( lNode.Type == VarType.NUMBER_ARRAY )
                {
                    var dict = new Dictionary<int, double>();
                    if (nodeDict != null)
                    {
                        foreach (KeyValuePair<int, ValueNode> entry in nodeDict)
                        {
                            var entryValue = EvaluateValueNode(entry.Value, out var entryType, true);
                            if (entryType != expectedType) throw new Exception("Expected " + expectedType + ", got " + entryType);
                            dict[entry.Key] = (double)entryValue;
                        }
                    }
                    resultType = VarType.NUMBER_ARRAY;
                    return dict;
                }
                if( lNode.Type == VarType.STRING_ARRAY )
                {
                    var dict = new Dictionary<int, string>();
                    if (nodeDict != null)
                    {
                        foreach (KeyValuePair<int, ValueNode> entry in nodeDict)
                        {
                            var entryValue = EvaluateValueNode(entry.Value, out var entryType, true);
                            if (entryType != expectedType) throw new Exception("Expected " + expectedType + ", got " + entryType);
                            dict[entry.Key] = (string)entryValue;
                        }
                    }
                    resultType = VarType.STRING_ARRAY;
                    return dict;
                }

                throw new NotImplementedException("Unknown type " + lNode.Type);
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

                    if( variable.Type == VarType.BOOLEAN_ARRAY )
                    {
                        var value = (variable.Value as Dictionary<int, bool>)![Convert.ToInt32(index)];
                        resultType = VarType.BOOLEAN;
                        return value;
                    }
                    if( variable.Type == VarType.NUMBER_ARRAY )
                    {
                        var value = (variable.Value as Dictionary<int, double>)![Convert.ToInt32(index)];
                        resultType = VarType.NUMBER;
                        return value;
                    }
                    if( variable.Type == VarType.STRING_ARRAY )
                    {
                        var value = (variable.Value as Dictionary<int, string>)![Convert.ToInt32(index)];
                        resultType = VarType.STRING;
                        return value;
                    }
                    if( variable.Type == VarType.STRING )
                    {
                        var value = (variable.Value as string)![Convert.ToInt32(index) - 1];
                        resultType = VarType.CHAR;
                        return value;
                    }
                }
            }
            else if (Utilities.IsSameClass(node.GetType(), typeof(BinaryExpressionNode)))
            {
                var bNode = (BinaryExpressionNode)node;
                var left = EvaluateValueNode(bNode.Left, out var leftType, local);
                var right = EvaluateValueNode(bNode.Right, out var rightType, local);

                if (left == null || right == null) return null;

                if( bNode.Operator == BinaryExpressionOperator.ADD )
                {
                    if( leftType == VarType.STRING || rightType == VarType.STRING ) {
                        resultType = VarType.STRING;
                        return (dynamic)left + (dynamic)right;
                    }
                }

                if (leftType != rightType) throw new Exception("Type mismatch");
                if (Utilities.IsTypeArray(leftType)) throw new Exception("Binary expression of an array");

                if (bNode.Type == BinaryExpressionType.ARITHMETIC && leftType != VarType.NUMBER) throw new Exception("Expected type double in arithmetic expression.");
                if( bNode.Type == BinaryExpressionType.RELATIONAL )
                {
                    // TODO: Type checks
                }

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
                    BinaryExpressionOperator.GTE => (double)left >= (double)right,
                    BinaryExpressionOperator.LTE => (double)left <= (double)right,
                    BinaryExpressionOperator.GT => (double)left > (double)right,
                    BinaryExpressionOperator.LT => (double)left < (double)right,
                    BinaryExpressionOperator.NEQ => !left.Equals(right),
                    BinaryExpressionOperator.EQ => left.Equals(right),
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

            if (Utilities.IsSameClass(node.GetType(), typeof(IdentifierNode))) ThrowRuntimeError(node, "Unknown variable: " + ((IdentifierNode)node).Identifier);
            else ThrowRuntimeError(node, "Unknown value: " + node);
            throw new Exception();
        }

        internal void EvalauateStatementsNode(StatementsNode node)
        {
            uint createdVariables = 0;

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
                    createdVariables++;
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
                if( Utilities.IsSameClass(statement.GetType(), typeof(ArrayModifyNode)))
                {
                    var amNode = (ArrayModifyNode)statement;
                    Variable? var = Variables.Get(amNode.Identifier!.Identifier, true);

                    if( var == null ) ThrowRuntimeError(amNode, "Variable " + amNode.Identifier!.Identifier + " not found.");

                    var indexValue = EvaluateValueNode(amNode.Identifier.Index, out var indexType, true);
                    if (indexType != VarType.NUMBER) ThrowRuntimeError(amNode.Identifier!.Index!, "Expected type " + VarType.NUMBER + ", got " + indexType);

                    var value = EvaluateValueNode(amNode.Value, out var valueType, true);
                    if( valueType != Utilities.GetArrayBaseType(var.Type)) ThrowRuntimeError(amNode, "Expected type " + Utilities.GetArrayBaseType(var.Type) + ", got " + valueType);

                    if( var.Type == VarType.BOOLEAN_ARRAY ) { (var.Value as Dictionary<int, bool>)![Convert.ToInt32(indexValue)] = Convert.ToBoolean(value); }
                    if( var.Type == VarType.NUMBER_ARRAY ) { (var.Value as Dictionary<int, double>)![Convert.ToInt32(indexValue)] = Convert.ToDouble(value); }
                    if( var.Type == VarType.STRING_ARRAY ) { (var.Value as Dictionary<int, string>)![Convert.ToInt32(indexValue)] = Convert.ToString(value); }
                }
            }

            Variables.Pop(false, createdVariables);
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
