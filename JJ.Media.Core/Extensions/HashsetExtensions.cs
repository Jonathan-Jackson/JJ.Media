using System.Collections.Generic;

namespace JJ.Media.Core.Extensions {

    public static class HashsetExtensions {

        public static bool TryAdd<T>(this HashSet<T> set, T value) {
            if (set.Contains(value)) {
                return false;
            }

            set.Add(value);
            return true;
        }

        public static void TryAdd<T>(this HashSet<T> set, IEnumerable<T> values) {
            foreach (var value in values) {
                if (!set.Contains(value)) {
                    set.Add(value);
                }
            }
        }
    }
}