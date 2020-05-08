using JJ.Media.Core.Extensions;
using MediaInfo.Domain.Helpers.DTOs.Miners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaInfo.DomainLayer.Miners {

    /// <summary>
    /// Searches through a string commonly associated with anime to return information relating to media.
    /// </summary>
    public class AnimeMiner : Miner, IMiner {

        public MinedEpisode MineEpisodeName(string episodeName) {
            if (episodeName == null)
                throw new ArgumentNullException(episodeName);
            if (string.IsNullOrWhiteSpace(episodeName))
                throw new ArgumentException(nameof(episodeName));

            episodeName = episodeName.RemoveAtEnd(SupportedMediaFormats, StringComparison.OrdinalIgnoreCase);

            return new MinedEpisode {
                PossibleNames = GetPossibleNames(episodeName).ToArray(),
                EpisodeNumber = GetEpisodeNumber(episodeName),
                SeasonNumber = GetSeasonNumber(episodeName),
                Source = episodeName
            };
        }

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

        private int GetEpisodeNumber(string episodeName) {
            // Try removing metadata first to stop "[1080p-FLAC]" breaking the hyphen splitting
            string metadataRemoved = RemoveCircularBrackets(RemoveSquareBrackets(episodeName));

            string removedShow = metadataRemoved.Split('-').Count() > 1
                                    ? metadataRemoved.Split('-').Last()
                                    : episodeName.Split('-').Last();

            string noLetters = removedShow.RemoveLetters(replace: ' ');
            return noLetters.Split(' ')
                .Where(x => int.TryParse(x, out _))
                .Select(x => int.Parse(x))
                .LastOrDefault();
        }

        private IEnumerable<string> GetPossibleNames(string episodeName) {
            // 4! = 24 combinations
            var priorityPermutations = new List<Func<string, string>> {
                RemoveSquareBrackets,
                RemoveTriangleBrackets,
                SpliceOnHyphen
            }.GetPermutations().ToArray();

            // 5! = 120 combinations
            var secondary = new List<Func<string, string>> {
                SpliceOnHyphen,
                RemoveSeasonLetter,
                RemoveRomanNumerals,
                RemoveBuzzwords,
                RemoveSpecialSeasonWords
            };
            var secondaryPermutations = secondary.GetPermutations()
                // add an extra sequence to remove circular brackets (some shows have a year on the end).
                .Concat(new[] { secondary.Append(RemoveCircularBrackets) })
                .ToList();

            // 2! = 2 combinations
            var tertiaryPermutations = new List<Func<string, string>> {
                RemoveNumbersAtEnd,
                RemoveLastWord
            }.GetPermutations().ToArray();

            HashSet<string> results = new HashSet<string>();

            // This needs better comments and refactoring.
            // In Short:
            // We randomize our functions with permutations, our secondary
            // functions we want to execute each individually, not just all.
            foreach (var priorityActions in priorityPermutations) {
                string result1 = TryAggregateActions(episodeName, priorityActions).Trim();
                results.TryAdd(result1);
                results.TryAdd(secondary.Select(x => x(result1)));

                foreach (var secondaryActions in secondaryPermutations) {
                    var secondarySequence = priorityActions.Concat(secondaryActions).ToArray();
                    string result2 = TryAggregateActions(episodeName, secondarySequence).Trim();
                    results.TryAdd(result2);

                    foreach (var tertiaryActions in tertiaryPermutations) {
                        var tertiarySequence = secondarySequence.Concat(tertiaryActions).ToArray();
                        string result3 = TryAggregateActions(episodeName, tertiarySequence).Trim();
                        results.TryAdd(result3);
                    }
                }
            }

            return results
                .Where(x => x.Length > 2 || x.Length == episodeName.Length)
                .Select(x => x.Trim())
                // Prioritize the longest values
                // We do this because going down the permutations will only result
                // in small results - and these are commonly less likely.
                .OrderByDescending(x => x)
                .Distinct()
                .DefaultIfEmpty(episodeName)
                .ToArray();
        }

        private int GetRomanNumeralSeason(string arg) {
            // Anything above these is 99% a season name!
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

        private int GetSeasonNotation(string arg) {
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
                    return int.Parse(seasonNumbers);
                }
                else {
                    fileNameCopy = fileNameCopy.Substring(index + 2, fileNameCopy.Length - index - 2);
                }
            }

            return 0;
        }

        private int? GetSeasonNumber(string episodeName) {
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

        private string RemoveSeasonLetter(string arg) {
            var sentence = new List<string>();
            foreach (string word in arg.Split(' ')) {
                string change = RemoveNumbersAtEnd(word);
                change = change.RemoveAtEnd("s", StringComparison.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(change))
                    sentence.Add(word);
            }

            return string.Join(' ', sentence);
        }

        private string RemoveSpecialSeasonWords(string arg) {
            return arg.Replace(" OVA ", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace(" special ", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace(" oad ", string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}