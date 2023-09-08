using System;
using System.Collections.Generic;
using System.Text;

namespace OTS_Console.Util
{
    internal static class StringParser
    {
        /// <summary>Parse the given string, separating by the given delimiter, ignoring those if it's between two qualifiers</summary>
        /// <param name="line">text to parse</param>
        /// <param name="delimiter">character to split the text by</param>
        /// <param name="textQualifier">character to ignore the delimiter if between a pair of these</param>
        /// <returns>a collection of strings, separated by the delimiter (when not between to qualifiers)</returns>
        public static IEnumerable<String> ParseText(String line, Char delimiter = ' ', Char textQualifier='"')
        {//original version by psubsee2003 on stackoverflow  https://stackoverflow.com/a/14655185

            if (line == null || string.IsNullOrWhiteSpace(line))//bodygaurd
            { yield break; }
            else
            {
                Char prevChar = '\0';
                Char nextChar = '\0';
                Char currentChar = '\0';

                Boolean inString = false;

                StringBuilder token = new StringBuilder();

                for (int i = 0; i < line.Length; i++)
                {
                    currentChar = line[i];

                    if (i > 0)
                        prevChar = line[i - 1];
                    else
                        prevChar = '\0';

                    if (i + 1 < line.Length)
                        nextChar = line[i + 1];
                    else
                        nextChar = '\0';

                    //check for if we reached a correct string
                    if (currentChar == textQualifier && (prevChar == '\0' || prevChar == delimiter) && !inString)
                    {
                        inString = true;
                        continue;
                    }
                    //check for if we're leaving a correct string
                    if (currentChar == textQualifier && (nextChar == '\0' || nextChar == delimiter) && inString)
                    {
                        inString = false;
                        continue;
                    }

                    if (currentChar == delimiter && !inString)
                    {//what is this doing???????
                        yield return token.ToString();
                        token = token.Remove(0, token.Length);
                        continue;
                    }

                    token = token.Append(currentChar);
                }
                //return all the found texts in an IEnumerable
                yield return token.ToString();

            }
        }
    }
}
