namespace fim.spike.Nodes
{
    public class LiteralNode : ValueNode
    {
        public string RawValue = "";
        public object Value
        {
            get
            {
                if( Type == VarType.STRING ) { return RawValue[1..^1]; }
                if( Type == VarType.CHAR ) { return char.Parse(RawValue[1..^1]); }
                if( Type == VarType.BOOLEAN ) { return Array.IndexOf(new string[] { "yes", "true", "right", "correct" }, RawValue) != -1;  }
                if( Type == VarType.NUMBER ) { return double.Parse(RawValue); }

                return RawValue;
            }
            set
            {
                RawValue = Convert.ToString(value)!;
            }
        }
        public VarType Type = VarType.UNKNOWN;
    }
}
