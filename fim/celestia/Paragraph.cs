using fim.spike.Nodes;

namespace fim.celestia
{
    public class Paragraph
    {
        private Interpreter _interpreter;
        private FunctionNode _node;
        public readonly string Name;
        public readonly bool Main;

        internal Paragraph(Interpreter interpreter, FunctionNode node)
        {
            _interpreter = interpreter;
            _node = node;
            Name = node.Name;
            Main = node.IsMain;
        }

        public void Execute()
        {
            _interpreter.Variables.PushFunctionStack();

            if(_node.Statements != null)
            {
                _interpreter.EvalauateStatementsNode(_node.Statements);
            }

            _interpreter.Variables.PopFunctionStack();
        }
    }
}
