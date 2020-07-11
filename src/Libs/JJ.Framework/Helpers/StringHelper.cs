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

        public static string GetWordAtIndex(string value, int index) {
            if (index < 0)
                throw new ArgumentException("Index value must be greater than zero.");
            if (index > value.Length - 1)
                throw new ArgumentException($"Index ({index}) is greater than the length of the value ({value}).");
            if (value[index] == ' ')
                return " ";

            int suffix = value.Substring(index).IndexOf(' ');
            int prefix = value.Substring(0, index + 1).LastIndexOf(' ') + 1;

            if (suffix > -1 && prefix > 0) {
                suffix += index;
                suffix -= prefix;
                return value.Substring(prefix, suffix);
            }
            else if (suffix > -1) {
                suffix += index;
                return value.Substring(0, suffix);
            }
            else if (prefix > 0) {
                return value.Substring(prefix);
            }
            else {
                return value;
            }
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