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
    }
}