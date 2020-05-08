using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediaInfo.DomainLayer.Miners {

    /// <summary>
    /// Rips information from a string.
    /// </summary>
    public abstract class Miner {

        protected static readonly string[] SupportedMediaFormats = new[] {
            ".mkv",
            ".mp4"
        };

        protected static readonly char[] SupportedRomanNumerals = new[] { 'i', 'v', 'I', 'V' };

        protected virtual bool ContainsRomanNumerals(string arg)
            // Only do realistically seasoned roman'd
            => arg.Contains(" I ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" II ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" III ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" IV ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" V ", StringComparison.OrdinalIgnoreCase);

        protected string RemoveCircularBrackets(string arg)
            => Regex.Replace(arg, @" ?\(.*?\)", string.Empty);

        protected string RemoveLastWord(string arg) {
            var splitSpaces = arg.Split(' ');

            if (splitSpaces.Length <= 1)
                return arg;

            string result = string.Empty;
            for (var i = 0; i < splitSpaces.Length - 1; i++) {
                result += splitSpaces[i] + " ";
            }

            return result.TrimEnd();
        }

        protected string RemoveNumbersAtEnd(string arg) {
            string reversed = string.Concat(arg.Reverse());

            for (int i = 0; i < reversed.Length; i++) {
                if (char.IsLetter(reversed[i]))
                    return arg.Substring(0, arg.Length - i);
            }

            return arg;
        }

        protected string RemoveRomanNumerals(string arg)
            => arg.Replace(" i ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" ii ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" iii ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" v ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" iv ", string.Empty, StringComparison.OrdinalIgnoreCase);

        protected string RemoveSquareBrackets(string arg)
            => Regex.Replace(arg, @" ?\[.*?\]", string.Empty);

        protected string RemoveTriangleBrackets(string arg)
            => Regex.Replace(arg, @" ?\<.*?\>", string.Empty);

        protected string SpliceOnHyphen(string arg) {
            var split = arg.Split('-');

            if (split.Length == 1)
                return arg;

            return string.Concat(string.Join('-', split.Take(split.Length - 1)));
        }

        protected bool StartsWithNumberedWord(string fileName) {
            if (fileName.Length == 0)
                return false;

            for (int i = 0; i < fileName.Length; i++) {
                if (char.IsDigit(fileName[i]))
                    continue;
                else if (i > 0 && fileName[i] == ' ')
                    return true;
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Attempts to aggregate a collection of functions against a source string.
        /// If an exception is thrown, it is caught.
        /// </summary>
        protected virtual string TryAggregateActions(string source, IEnumerable<Func<string, string>> actions) {
            try {
                foreach (var action in actions) {
                    source = action(source);
                }
                return source;
            }
            catch {
                return source;
            }
        }
    }
}