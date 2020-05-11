using System;
using System.Collections.Generic;
using System.Linq;

namespace JJ.Media.Core.Extensions {

    public static class StringExtensions {

        public static string RemoveLetters(this string value, char replace)
            => new string(value.Select(x => !char.IsLetter(x) ? x : replace).ToArray());

        public static string RemoveLetters(this string value)
            => new string(value.Where(x => !char.IsLetter(x)).ToArray());

        public static string RemoveDigits(this string value, char replace)
            => new string(value.Select(x => !char.IsDigit(x) ? x : replace).ToArray());

        public static string RemoveDigits(this string value)
            => new string(value.Where(x => !char.IsDigit(x)).ToArray());

        public static string RemoveAtEnd(this string value, IEnumerable<string> endValues, StringComparison comparison = StringComparison.Ordinal) {
            foreach (var end in endValues) {
                if (value.EndsWith(end, comparison))
                    return value.Substring(0, value.Length - end.Length);
            }

            return value;
        }

        public static string RemoveAtEnd(this string value, string endValue, StringComparison comparison = StringComparison.Ordinal)
            => RemoveAtEnd(value, new[] { endValue }, comparison);
    }
}