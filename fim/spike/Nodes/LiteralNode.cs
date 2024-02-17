using System.Text;

namespace fim.spike.Nodes
{
    public class LiteralNode : ValueNode
    {
        public string RawValue = "";
        public object Value
        {
            get
            {
                if( Type == VarType.STRING ) {
                    return Utilities.UnsanitizeString(RawValue, true);
                }
                if( Type == VarType.CHAR ) {
                    string expectedChar = RawValue[1..^1];
                    if( expectedChar.StartsWith("\\") ) {
                        switch(expectedChar[1])
                        {
                            case '0': return '\0';
                            case 'r': return '\r';
                            case 'n': return '\n';
                            case 't': return '\t';
                            default: return expectedChar[1];
                        } 
                    }
                    return char.Parse(expectedChar);
                }
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

    public class LiteralDictNode : ValueNode
    {
        public Dictionary<int, ValueNode> RawDict = new Dictionary<int, ValueNode>();

        public object Value
        {
            get
            {
                return RawDict;
            }
            set
            {
                RawDict = (value as IDictionary<int, ValueNode>).ToDictionary(i => (int)i.Key, i => (ValueNode)i.Value);
            }
        }

        public VarType Type = VarType.UNKNOWN;
    }
}
