using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fim 
{
    internal class Utilities
    {
        public static bool IsStringNumber(string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }
        public static bool IsIndentCharacter(char c) { return c == ' ' || c == '\t'; }
        public static bool IsIndentCharacter(string c ) {  return c.Length == 1 && IsIndentCharacter(char.Parse(c)); }
    }
}
