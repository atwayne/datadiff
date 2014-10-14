using System;
using System.Collections.Generic;
using System.Text;

namespace LastR2D2.Tools.DataDiff.Core
{
    public static class ExtensionMethods
    {
        public static string TrimWhiteSpaces(this string text, HashSet<char> toExclude)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (toExclude == null)
                throw new ArgumentNullException("toExclude");

            var builder = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                if (!toExclude.Contains(c))
                    builder.Append(c);
            }
            return builder.ToString();
        }

        public static string TrimWhiteSpaces(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return text.TrimWhiteSpaces(new HashSet<char>(new[] { ' ', '\t', '\n', '\r' }));
        }

        public static IList<string> BreakIntoList(this string text, int maxCharacters)
        {
            if (text == null)
                return null;

            if (text.Length == 0)
                return new List<string>();

            var start = 0;
            var totalLength = text.Length;

            var result = new List<string>();
            do
            {
                var step = totalLength - start;
                if (maxCharacters < step)
                    step = maxCharacters;

                var piece = text.Substring(start, step);
                start += step;
                result.Add(piece);
            } while (start < totalLength);

            return result;
        }
    }
}