using System;
using System.Collections.Generic;
using System.Linq;

namespace JJ.Framework.Helpers {

    public static class StringHelper {

        public static IEnumerable<int> GetIndices(string value, char match, StringComparison comparer) {
            for (int i = 0; i < value.Length; i++)
                if (string.Equals(value[i].ToString(), match.ToString(), comparer))
                    yield return i;
        }

        public static string RemoveLetters(string value, char replace)
            => new string(value.Select(x => !char.IsLetter(x) ? x : replace).ToArray());

        public static string RemoveLetters(string value)
            => new string(value.Where(x => !char.IsLetter(x)).ToArray());

        public static string RemoveDigits(string value, char replace)
            => new string(value.Select(x => !char.IsDigit(x) ? x : replace).ToArray());

        public static string RemoveDigits(string value)
            => new string(value.Where(x => !char.IsDigit(x)).ToArray());

        public static string RemoveAtEnd(string value, IEnumerable<string> endValues, StringComparison comparison = StringComparison.Ordinal) {
            foreach (var end in endValues) {
                if (value.EndsWith(end, comparison))
                    return value.Substring(0, value.Length - end.Length);
            }

            return value;
        }

        public static string RemoveAtEnd(string value, string endValue, StringComparison comparison = StringComparison.Ordinal)
            => RemoveAtEnd(value, new[] { endValue }, comparison);
    }
}