using fim.spike;
using fim.spike.Nodes;
using System.Data;

namespace fim.celestia
{
    public class Paragraph
    {
        private Interpreter _interpreter;
        private FunctionNode _node;

        public string Name { get { return _node.Name; } }
        public bool Main { get { return _node.IsMain; } }
        public VarType? Returns { get { return _node.ReturnType; } }
        public List<FunctionNode.FunctionNodeParameter>? Parameters { get { return _node.ParametersType; } }

        internal Paragraph(Interpreter interpreter, FunctionNode node)
        {
            _interpreter = interpreter;
            _node = node;
        }

        public ValueNode? Execute(List<ValueNode>? parameters = null)
        {
            _interpreter.Variables.PushFunctionStack();

            if( parameters != null )
            {
                if ( Parameters == null ) { _interpreter.ThrowRuntimeError(_node, "Paragraph with no parameters expected parameters."); }

                int i = 0;
                while(i < parameters.Count)
                {
                    if( i >= Parameters!.Count ) { _interpreter.ThrowRuntimeError(_node, "Paragraph parameters are overloaded."); }

                    var paramInput = parameters[i];
                    var value = _interpreter.EvaluateValueNode(paramInput, out VarType valueType);

                    var paragraphParameter = Parameters[i];
                    if( paragraphParameter.Type != valueType ) { _interpreter.ThrowRuntimeError(paramInput, "Expected type " + paragraphParameter.Type + ", got " + valueType); }

                    _interpreter.Variables.Push(new Variable(paragraphParameter.Name, value, valueType));

                    i++;
                }

                while(i < Parameters!.Count)
                {
                    var paragraphParameter = Parameters[i];

                    var defaultValueNode = AST.CreateValueNode(new List<twilight.Token>(), possibleNullType: paragraphParameter.Type);
                    var result = _interpreter.EvaluateValueNode(defaultValueNode, out VarType resultType);

                    if( paragraphParameter.Type != resultType ) { _interpreter.ThrowRuntimeError(_node, "Expected type " + paragraphParameter.Type + ", got " + resultType); }

                    _interpreter.Variables.Push( new Variable( paragraphParameter.Name, result, resultType ) );

                    i++;
                }
            }

            ValueNode? returnValue = null;
            if (_node.Body != null)
            {
                returnValue = _interpreter.EvalauateStatementsNode(_node.Body);
            }

            _interpreter.Variables.PopFunctionStack();

            if( Returns == null && returnValue != null )
            {
                _interpreter.ThrowRuntimeError(_node, "Function with no return type returned a value.");
            }

            return returnValue;
        }
    }
}
