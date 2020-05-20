using System;
using System.Runtime.Serialization;

namespace Storage.Domain.Helpers.Exceptions {

    [Serializable]
    public class EpisodeNotFoundException : Exception {

        public EpisodeNotFoundException() {
        }

        public EpisodeNotFoundException(string message) : base(message) {
        }

        public EpisodeNotFoundException(string message, Exception innerException) : base(message, innerException) {
        }

        protected EpisodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}