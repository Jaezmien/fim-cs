using fim.spike;

namespace fim.celestia
{
    public class Variable
    {
        public readonly string Name;
        private object? _Value;
        internal VarType Type;
        public readonly bool IsConstant;

        internal Variable(string name, object? value, VarType type, bool isConstant = false)
        {
            Name = name;
            _Value = value;
            Type = type; 
            IsConstant = isConstant;
        }

        public object? Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if( value.GetType() != _Value.GetType() ) throw new ArgumentException( "Mismatch value types" );

                _Value = value;
            }
        }
    }
    public class VariableManager
    {
        public readonly Stack<Variable> GlobalVariables;
        public readonly Stack<Stack<Variable>> LocalVariables;

        public VariableManager()
        {
            GlobalVariables = new Stack<Variable>();
            LocalVariables = new Stack<Stack<Variable>>();
        }

        public int StackDepth { get { return LocalVariables.Count; } }

        public void Push(Variable var, bool isGlobal = false)
        {
            if (isGlobal) this.GlobalVariables.Push(var);
            else this.LocalVariables.Peek().Push(var);
        }
        public Variable Pop(bool isGlobal = false)
        {
            if (isGlobal) return this.GlobalVariables.Pop();
            else return this.LocalVariables.Peek().Pop();
        }
        public Variable[] Pop(bool isGlobal = false, uint count = 1)
        {
            Stack<Variable> variables = new();
            Stack<Variable> stack = isGlobal ? this.GlobalVariables : this.LocalVariables.Peek();

            while (count > 0 && stack.Count > 0)
            {
                variables.Push(stack.Pop());
                count--;
            }

            return variables.ToArray();
        }

        public void PushFunctionStack() => this.LocalVariables.Push(new());
        public void PopFunctionStack() => this.LocalVariables.Pop();

        public bool Has(string name, bool local = true) => this.Get(name, local) != null;
        internal Variable? Get(string name, bool local = true)
        {
            var variable = this.GlobalVariables.FirstOrDefault(v => v.Name == name);
            if (variable != null) return variable;

            if (local && this.LocalVariables.Count > 0)
            {
                variable = this.LocalVariables.Peek().FirstOrDefault(v => v.Name == name);
                if (variable != null) return variable;
            }

            return null;
        }
        public Variable? Get(string name)
        {
            return this.Get(name, false);
        }
    }
}
