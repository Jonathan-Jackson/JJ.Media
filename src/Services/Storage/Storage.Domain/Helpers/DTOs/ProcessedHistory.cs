using JJ.Media.Core.Entities;
using System;

namespace Storage.Domain.Helpers.DTOs {

    public class ProcessedHistory : Entity {
        public eProcessedType Type { get; set; }

        public string Source { get; set; } = string.Empty;

        public string Output { get; set; } = string.Empty;

        public DateTimeOffset ProcessedOn { get; set; } = DateTimeOffset.UtcNow;
    }

    public enum eProcessedType {
        Unknown = 0,
        Episode = 1
    }
}