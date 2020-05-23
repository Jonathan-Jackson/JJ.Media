using Storage.Domain.Helpers.DTOs;
using System;

namespace Storage.Domain.Helpers.Options {

    public class MediaStorageOptions {
        public StoreArea[] Stores { get; set; } = Array.Empty<StoreArea>();

        public StoreArea[] ProcessStores { get; set; } = Array.Empty<StoreArea>();
    }
}