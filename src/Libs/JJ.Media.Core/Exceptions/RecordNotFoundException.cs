using System;

namespace JJ.Media.Core.Models.Exceptions {

    [Serializable]
    public class RecordNotFoundException : Exception {

        public RecordNotFoundException() {
        }

        public RecordNotFoundException(int id)
            : base($"Id: {id}") {
        }
    }
}