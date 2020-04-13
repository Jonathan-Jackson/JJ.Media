using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace JJ.Media.MediaInfo.Services.Miners {

    public class AnimeMiningService : IMiningService {
        private readonly ILogger _logger;

        public AnimeMiningService(ILogger logger) {
            _logger = logger;
        }

        public MinedEpisode MineEpisodeName(string episodeName) {
            return new MinedEpisode {
                PossibleNames = GetPossibleNames(episodeName).ToArray(),
                EpisodeNumber = GetEpisodeNumber(episodeName),
                SeasonNumber = GetSeasonNumber(episodeName)
            };
        }

        private string AggregateActions(string source, IEnumerable<Func<string, string>> actions) {
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

        private uint GetEpisodeNumber(string episodeName) {
            throw new NotImplementedException();
        }

        private IEnumerable<string> GetPossibleNames(string episodeName) {
            var actions = new List<Func<string, string>> {
                RemoveSquareBrackets,
                SpliceOnHyphen,
                RemoveCircularBrackets,
                RemoveTriangleBrackets,
                SpliceOnSecondHyphen,
                RemoveSeasonLetter,
                RemoveRomanNumerals,
                RemoveBuzzwords,
                RemoveSpecialSeasonWords,
                RemoveNumberAtEnd,
                RemoveLastWord
            };

            var poweredActions = PowerSets(actions);

            string[] results = poweredActions
                .Select(x => AggregateActions(episodeName, x))
                .Where(x => x.Length > 2)
                .Distinct()
                .OrderByDescending(x => x.Length)
                .ToArray();

            if (results.Length == 1) {
                _logger.LogError($"Failed to mine show names from: '{episodeName}'.");
            }

            return results;
        }

        private uint GetSeasonNumber(string episodeName) {
            throw new NotImplementedException();
        }

        private IEnumerable<IEnumerable<T>> PowerSets<T>(IList<T> set) {
            var totalSets = BigInteger.Pow(2, set.Count);
            for (BigInteger i = 0; i < totalSets; i++) {
                yield return SubSet(set, i);
            }
        }

        private string RemoveBuzzwords(string arg) {
            return arg.Replace(" ACT ", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private string RemoveCircularBrackets(string arg)
            => Regex.Replace(arg, @" ?\(.*?\)", string.Empty);

        private string RemoveLastWord(string arg) {
            var splitSpaces = arg.Split(' ');

            if (splitSpaces.Length <= 1)
                return arg;

            string result = string.Empty;
            for (var i = 0; i < arg.Length - 1; i++) {
                result += arg[i];
            }

            return result;
        }

        private string RemoveNumberAtEnd(string arg) {
            string reversed = string.Concat(arg.Reverse());

            for (int i = 0; i < reversed.Length; i++) {
                if (char.IsLetter(reversed[i]))
                    return arg.Substring(0, arg.Length - i);
            }

            return arg;
        }

        private string RemoveRomanNumerals(string arg)
            => arg.Replace(" i ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" ii ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" iii ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" v ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(" iv ", string.Empty, StringComparison.OrdinalIgnoreCase);

        private string RemoveSeasonLetter(string arg) {
            throw new NotImplementedException();
        }

        private string RemoveSpecialSeasonWords(string arg) {
            return arg.Replace(" OVA ", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace(" special ", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace(" oad ", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private string RemoveSquareBrackets(string arg)
            => Regex.Replace(arg, @" ?\[.*?\]", string.Empty);

        private string RemoveTriangleBrackets(string arg)
                    => Regex.Replace(arg, @" ?\<.*?\>", string.Empty);

        private string SpliceOnHyphen(string arg) {
            var split = arg.Split('-');

            if (split.Length == 1)
                return arg;

            return string.Concat(split.Take(split.Length - 1));
        }

        private string SpliceOnSecondHyphen(string arg) {
            var split = arg.Split('-');

            if (split.Length < 3)
                return arg;

            return string.Concat(split.Take(split.Length - 2));
        }

        private IEnumerable<T> SubSet<T>(IList<T> set, BigInteger n) {
            for (int i = 0; i < set.Count && n > 0; i++) {
                if ((n & 1) == 1) {
                    yield return set[i];
                }

                n = n >> 1;
            }
        }
    }
}