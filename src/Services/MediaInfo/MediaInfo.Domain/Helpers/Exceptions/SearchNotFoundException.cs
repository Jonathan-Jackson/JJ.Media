using System;

namespace MediaInfo.Domain.Helpers.Exceptions {

    public class SearchNotFoundException : Exception {

        public SearchNotFoundException(string message) : base(message) {
        }

        public SearchNotFoundException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}