using JJ.Media.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.Core.Extensions {

    public static class LinqExtensions {

        public static IEnumerable<int> GetIds<TEntity>(this IEnumerable<TEntity> enumerable)
            where TEntity : Entity
            => enumerable.Select(x => x.Id);

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