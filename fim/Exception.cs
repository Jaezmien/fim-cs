using System;

namespace fim
{
    public class FiMException : Exception
    {
        public FiMException() : base() { }
        public FiMException(string message) : base(message) { }
        public FiMException(string message, Exception inner) : base(message, inner) { }

        public struct ExceptionPair
        {
            public int Line;
            public int Column;
        }
        /// <summary>
        /// Grabs the <c>ExceptionPair</c> of an index in a given content.<br/>Used when grabbing what line of FiM++ report has given an error.
        /// </summary>
        public static ExceptionPair GetIndexPair(string content, int index)
        {
            string subContent = content.Substring(0, index + 1);
            string[] lines = subContent.Split('\n');

            return new ExceptionPair()
            {
                Line = lines.Length,
                Column = lines[lines.Length - 1].Length
            };
        }
    }
}
