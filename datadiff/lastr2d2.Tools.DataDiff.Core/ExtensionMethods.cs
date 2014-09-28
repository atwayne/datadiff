using System.Collections.Generic;
using System.Text;

namespace lastr2d2.Tools.DataDiff.Core
{
    public static class ExtensionMethods
    {
        public static string TrimWhiteSpaces(this string str, HashSet<char> toExclude)
        {
            var builder = new StringBuilder(str.Length);
            foreach (var c in str)
            {
                if (!toExclude.Contains(c))
                    builder.Append(c);
            }
            return builder.ToString();
        }

        public static string TrimWhiteSpaces(this string str)
        {
            return str.TrimWhiteSpaces(new HashSet<char>(new[] { ' ', '\t', '\n', '\r' }));
        }

        public static List<string> BreakIntoList(this string str, int maxCharacters)
        {
            var start = 0;
            var totalLength = str.Length;

            var result = new List<string>();
            do
            {
                var step = totalLength - start;
                if (maxCharacters < step)
                    step = maxCharacters;

                var piece = str.Substring(start, step);
                start += step;
                result.Add(piece);
            } while (start < totalLength);

            return result;
        }
    }
}
