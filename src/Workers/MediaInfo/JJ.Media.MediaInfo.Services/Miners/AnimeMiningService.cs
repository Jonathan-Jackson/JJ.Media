using JJ.Media.Core.Extensions;
using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JJ.Media.MediaInfo.Services.Miners {

    public class AnimeMiningService : IMiningService {

        private static readonly string[] MediaFormats = new[] {
            ".mkv",
            ".mp4"
        };

        private readonly ILogger<AnimeMiningService> _logger;

        public AnimeMiningService(ILogger<AnimeMiningService> logger) {
            _logger = logger;
        }

        public MinedEpisode MineEpisodeName(string episodeName) {
            episodeName = episodeName.RemoveAtEnd(MediaFormats, StringComparison.OrdinalIgnoreCase);

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

        private bool ContainsRomanNumerals(string arg)
            // Only do realistically seasoned roman'd
            => arg.Contains(" I ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" II ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" III ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" IV ", StringComparison.OrdinalIgnoreCase)
                || arg.Contains(" V ", StringComparison.OrdinalIgnoreCase);

        private bool ContainsSeasonNotation(string arg) {
            int index = arg.IndexOf(" s", StringComparison.OrdinalIgnoreCase);

            if (index > 2 && !(index + 2 == arg.Length)) {
                return StartsWithNumberedWord(arg.Substring(index + 2, arg.Length - index - 2));
            }
            else {
                return false;
            }
        }

        private bool ContainsSpecialSeasonName(string fileName) {
            return fileName.Contains(" OVA ", StringComparison.OrdinalIgnoreCase)
                || fileName.Contains(" OAD ", StringComparison.OrdinalIgnoreCase)
                || fileName.Contains(" special ", StringComparison.OrdinalIgnoreCase);
        }

        private uint GetEpisodeNumber(string episodeName) {
            // Try removing metadata first to stop "[1080p-FLAC]" breaking the hyphen splitting
            string metadataRemoved = RemoveCircularBrackets(RemoveSquareBrackets(episodeName));

            string removedShow = metadataRemoved.Split('-').Count() > 1
                                    ? metadataRemoved.Split('-').Last()
                                    : episodeName.Split('-').Last();

            string noLetters = removedShow.RemoveLetters(replace: ' ');
            return (uint?)noLetters.Split(' ')
                .Where(x => int.TryParse(x, out _))
                .Select(x => int.Parse(x))
                .LastOrDefault() ?? 0;
        }

        private IEnumerable<string> GetPossibleNames(string episodeName) {
            // 4! = 24 combinations
            var priorityPermutations = new List<Func<string, string>> {
                RemoveSquareBrackets,
                RemoveCircularBrackets,
                RemoveTriangleBrackets,
                SpliceOnHyphen
            }.GetPermutations().ToArray();

            // 5! = 120 combinations
            var secondaryPermutations = new List<Func<string, string>> {
                SpliceOnSecondHyphen,
                RemoveSeasonLetter,
                RemoveRomanNumerals,
                RemoveBuzzwords,
                RemoveSpecialSeasonWords
            }.GetPermutations().ToArray();

            // 2! = 2 combinations
            var tertiaryPermutations = new List<Func<string, string>> {
                RemoveNumbersAtEnd,
                RemoveLastWord
            }.GetPermutations().ToArray();

            HashSet<string> results = new HashSet<string>();

            foreach (var priorityActions in priorityPermutations) {
                string result1 = AggregateActions(episodeName, priorityActions).Trim();
                results.TryAdd(result1);

                foreach (var secondaryActions in secondaryPermutations) {
                    var secondarySequence = priorityActions.Concat(secondaryActions).ToArray();
                    string result2 = AggregateActions(episodeName, secondarySequence).Trim();
                    results.TryAdd(result2);

                    foreach (var tertiaryActions in tertiaryPermutations) {
                        var tertiarySequence = priorityActions.Concat(secondaryActions).ToArray();
                        string result3 = AggregateActions(episodeName, secondarySequence).Trim();
                        results.TryAdd(result3);
                    }
                }
            }

            if (results.Count == 0) {
                _logger.LogError($"Failed to mine show names from: '{episodeName}'.");
            }

            return results
                .Select(x => x.Trim())
                .ToArray();
        }

        private uint GetRomanNumeralSeason(string arg) {
            if (arg.Contains(" I ", StringComparison.OrdinalIgnoreCase))
                return 1;
            else if (arg.Contains(" II ", StringComparison.OrdinalIgnoreCase))
                return 2;
            else if (arg.Contains(" III ", StringComparison.OrdinalIgnoreCase))
                return 3;
            else if (arg.Contains(" IV ", StringComparison.OrdinalIgnoreCase))
                return 4;
            else if (arg.Contains(" V ", StringComparison.OrdinalIgnoreCase))
                return 5;
            else
                return 0;
        }

        private uint GetSeasonNotation(string arg) {
            // Loop through until we've got no ' s' left.

            string fileNameCopy = arg;
            while (fileNameCopy.Length > 0) {
                int index = fileNameCopy.IndexOf(" s", StringComparison.OrdinalIgnoreCase);

                string seasonNumbers = string.Empty;
                for (int i = (index + 2); index != -1 && i < arg.Length; i++) {
                    if (char.IsDigit(arg[i])) {
                        seasonNumbers += arg[i];
                    }
                    else {
                        break;
                    }
                }

                if (seasonNumbers != string.Empty) {
                    return uint.Parse(seasonNumbers);
                }
                else {
                    fileNameCopy = fileNameCopy.Substring(index + 2, fileNameCopy.Length - index - 2);
                }
            }

            return 0;
        }

        private uint? GetSeasonNumber(string episodeName) {
            // Look for S2, S3.. in the title
            if (ContainsSeasonNotation(episodeName))
                return GetSeasonNotation(episodeName);
            else if (ContainsRomanNumerals(episodeName))
                return GetRomanNumeralSeason(episodeName);
            else if (ContainsSpecialSeasonName(episodeName))
                return 0;
            else // NOT FOUND.
                return null;
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

        private string RemoveNumbersAtEnd(string arg) {
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
            arg = RemoveNumbersAtEnd(arg);
            arg = arg.RemoveAtEnd("s", StringComparison.OrdinalIgnoreCase);

            var romanNumerals = new[] { 'i', 'v', 'I', 'V' };
            arg = arg.TrimEnd(romanNumerals);

            return arg;
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

        private bool StartsWithNumberedWord(string fileName) {
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
    }
}