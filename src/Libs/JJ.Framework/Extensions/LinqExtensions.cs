using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Framework.Extensions {

    public static partial class LinqExtensions {

        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
            => new Queue<T>(enumerable);

        public static T Random<T>(this IEnumerable<T> enumerable) {
            int rand = new Random().Next(0, enumerable.Count() - 1);
            return enumerable.ElementAt(rand);
        }

        public static async IAsyncEnumerable<T> WhereAsync<T>(this IEnumerable<T> enumerable, Func<T, Task<bool>> predicate) {
            foreach (var item in enumerable) {
                if (await predicate(item)) {
                    yield return item;
                }
            }
        }
    }
}