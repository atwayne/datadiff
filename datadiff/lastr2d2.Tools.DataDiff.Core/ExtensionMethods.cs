using System.Collections.Generic;
using System.Text;

namespace lastr2d2.Tools.DataDiff.Core
{
    internal static class ExtensionMethods
    {
        public static string TrimWhiteSpaces(this string str, HashSet<char> toExclude)
        {
            var sb = new StringBuilder(str.Length);
            foreach (var c in str)
            {
                if (!toExclude.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string TrimWhiteSpaces(this string str)
        {
            return str.TrimWhiteSpaces(new HashSet<char>(new[] { ' ', '\t', '\n', '\r' }));
        }
    }
}
