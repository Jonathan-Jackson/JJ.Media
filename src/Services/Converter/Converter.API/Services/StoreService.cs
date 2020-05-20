using Converter.API.Models;
using System.IO;
using System.Linq;

namespace Converter.API.Services {

    public class StoreService {
        private readonly StoreArea[] _stores;

        public StoreService(StoreArea[] stores) {
            _stores = stores;
        }

        public bool TryReplaceAlias(string value, out string result) {
            result = value;

            var store = _stores.FirstOrDefault(store =>
                                        value.StartsWith(store.Alias, System.StringComparison.OrdinalIgnoreCase));

            if (store == null)
                return false;

            result = store.Path + value.Substring(store.Alias.Length, value.Length - store.Alias.Length);
            return true;
        }
    }
}