namespace fim.spike.Nodes
{
    internal class LiteralNode : ValueNode
    {
        public string RawValue = "";
        public object Value
        {
            get
            {
                if( Type == VarType.STRING ) { return RawValue.Substring(1, RawValue.Length - 1); }
                if( Type == VarType.CHAR ) { return char.Parse(RawValue.Substring(1, RawValue.Length - 1)); }
                if( Type == VarType.BOOLEAN ) { return Array.IndexOf(new string[] { "yes", "true", "right", "correct" }, RawValue) != -1;  }
                if( Type == VarType.NUMBER ) { return double.Parse(RawValue); }

                return RawValue;
            }
            set
            {
                RawValue = value.ToString();
            }
        }
        public VarType Type = VarType.UNKNOWN;
    }
}
