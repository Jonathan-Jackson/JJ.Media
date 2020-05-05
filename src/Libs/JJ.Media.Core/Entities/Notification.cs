using System;

namespace JJ.Media.Core.Entities {

    public class Notification<TData> {

        public Notification() {
        }

        public Notification(TData data) {
            Data = data;
        }

        public TData Data { get; set; }
        public Guid Token { get; set; } = Guid.NewGuid();
    }
}