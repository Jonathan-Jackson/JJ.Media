using System.Collections.Generic;
using System.Numerics;

namespace JJ.Media.Core.Extensions {

    public static class ListExtensions {

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