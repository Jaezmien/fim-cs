using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fim.spike.Nodes
{
    public abstract class Node
    {
        public int Start;
        public int Length;

        public Node(): this(0, 0) { }
        public Node(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
    public abstract class ValueNode : Node
    {
        // Can be a LiteralNode, IdentifierNode, or a BinaryExpressionNode*
    }
}
