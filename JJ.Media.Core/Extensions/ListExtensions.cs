using System.Collections.Generic;
using System.Numerics;

namespace JJ.Media.Core.Extensions {

    public static class ListExtensions {

        /// <summary>
        /// Returns every possible sequence of the provided values, 2^ the set count.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> PowerOn<T>(this IList<T> set) {
            var totalSets = BigInteger.Pow(2, set.Count);
            for (BigInteger i = 0; i < totalSets; i++) {
                yield return SubSet(set, i);
            }
        }

        private static IEnumerable<T> SubSet<T>(IList<T> set, BigInteger n) {
            for (int i = 0; i < set.Count && n > 0; i++) {
                if ((n & 1) == 1) {
                    yield return set[i];
                }

                n = n >> 1;
            }
        }
    }
}