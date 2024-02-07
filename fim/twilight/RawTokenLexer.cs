using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fim.twilight
{
    internal class RawTokenLexer
    {
        internal readonly static char[] SPLITTABLE_CHARS = new char[] { '.', '!', '?', ':', ',', '(', ')', '"', '\'', ' ', '\t', '\\', '\n' };
        internal static Queue<RawToken> CreateRawTokens(string report)
        {
            Queue<RawToken> tokens = new();

            int start = 0;
            int current;
            while ((current = report.IndexOfAny(SPLITTABLE_CHARS, start)) != -1)
            {
                if (current - start > 0)
                {
                    tokens.Enqueue(new RawToken()
                    {
                        Start = start,
                        Length = current - start,
                        Value = report[start..current].Trim()
                    });
                }

                tokens.Enqueue(
                    new RawToken()
                    {
                        Start = current,
                        Length = 1,
                        Value = report.Substring(current, 1)
                    }
                );

                start = current + 1;
            }

            return tokens;
        }

        internal delegate bool ProcessResult(RawToken token, Queue<RawToken> oldTokens, out int dequeueAmount);
        internal static Queue<RawToken> MergeRawTokens(Queue<RawToken> oldTokens)
        {
            Queue<RawToken> newTokens = new();

            void ProcessToken(RawToken currentToken, ProcessResult condition)
            {
                if (!condition(currentToken, oldTokens, out int dequeueAmount)) return;
                
                MergeToRawToken(currentToken, oldTokens, dequeueAmount);
            }

            bool isNewLine = true;
            while( oldTokens.Count > 0 )
            {
                RawToken currentToken = oldTokens.Dequeue();

                // Trim starting newlines
                if (isNewLine && Utilities.IsIndentCharacter(currentToken.Value)) continue;
                isNewLine = false;

                ProcessToken(currentToken, CanMergeDecimal);
                ProcessToken(currentToken, CanMergeString);
                ProcessToken(currentToken, CanMergeChar);
                ProcessToken(currentToken, CanMergeDelimiters);

                if( currentToken.Length == 1 && currentToken.Value == "\n" )
                {
                    isNewLine = true;
                }

                newTokens.Enqueue(currentToken);
            }

            return newTokens;
        }
        internal static bool CanMergeDecimal(RawToken currentToken, Queue<RawToken> oldTokens, out int dequeueAmount)
        {
            dequeueAmount = -1;
            if( oldTokens.Count <= 1 ) { return false; }

            if( !Utilities.IsStringNumber(currentToken.Value) ) { return false; }

            RawToken pointToken = oldTokens.ElementAt(0);
            if( pointToken.Value != "." ) { return false;  }

            RawToken decimalToken = oldTokens.ElementAt(1);
            if(!Utilities.IsStringNumber(decimalToken.Value)) { return false; }

            dequeueAmount = 2;
            return true;
        }
        internal static bool CanMergeString(RawToken currentToken, Queue<RawToken> oldTokens, out int dequeueAmount)
        {
            const string DELIMITER = "\"";
            const string ESCAPE_TOKEN = "\\";

            dequeueAmount = -1;
            if( currentToken.Value != DELIMITER ) { return false;  }

            bool ignoreNextToken = false;
            int endTokenIndex = -1;
            for( int i = 0; i < oldTokens.Count; i++ )
            {
                RawToken preToken = oldTokens.ElementAt(i);
                if( preToken.Value == "\n" || i == oldTokens.Count - 1) { return false; }

                if( ignoreNextToken )
                {
                    ignoreNextToken = false;
                    continue;
                }

                if( preToken.Value == ESCAPE_TOKEN )
                {
                    ignoreNextToken = true;
                    continue;
                }

                if( preToken.Value == DELIMITER )
                {
                    endTokenIndex = i;
                    break;
                }
            }
            if (endTokenIndex == -1) { return false; }

            dequeueAmount = endTokenIndex + 1;
            return true;
        }
        internal static bool CanMergeChar(RawToken currentToken, Queue<RawToken> oldTokens, out int dequeueAmount)
        {
            const string DELIMITER = "\'";

            dequeueAmount = -1;
            if( currentToken.Value != DELIMITER ) { return false;  }

            if( oldTokens.Count > 2 && oldTokens.ElementAt(0).Value == "\\" && oldTokens.ElementAt(2).Value == DELIMITER ) {
                dequeueAmount = 3;
                return true;
            }
            if( oldTokens.Count > 1 && oldTokens.ElementAt(1).Value == DELIMITER ) {
                dequeueAmount = 2;
                return true;
            }

            return false;
        }
        internal static bool CanMergeDelimiters(RawToken currentToken, Queue<RawToken> oldTokens, out int dequeueAmount )
        {
            string[] START_DELIMITERS = { "(", "'" };
            string[] END_DELIMITERS = { ")", "'" };
            dequeueAmount = -1;

            int startDelimiterType = Array.IndexOf(START_DELIMITERS, currentToken.Value);
            if (startDelimiterType == -1) { return false; }

            int endDelimiterIndex = -1;
            for( int i = 0; i < oldTokens.Count; i++)
            {
                RawToken preToken = oldTokens.ElementAt(i);

                if( preToken.Value == "\n" || i == oldTokens.Count - 1) { return false; }

                int endDelimiterType = Array.IndexOf(END_DELIMITERS, preToken.Value);
                if( startDelimiterType != endDelimiterType ) { continue; }

                endDelimiterIndex = i;
                break;
            }
            if (endDelimiterIndex == -1) { return false; }

            // XXX: Expecting chars to only have one character.
            if( startDelimiterType == 1 && endDelimiterIndex != 1 ) { return false; }

            dequeueAmount = endDelimiterIndex + 1;
            return true;
        }

        internal static void MergeToRawToken(RawToken currentToken, Queue<RawToken> oldTokens, int amount)
        {
            while (amount > 0)
            {
                RawToken preToken = oldTokens.Dequeue();
                amount--;

                currentToken.Value += preToken.Value;
                if (amount == 0)
                {
                    currentToken.Length = preToken.Start + preToken.Length - currentToken.Start;
                }
            }
        }

    }
}
