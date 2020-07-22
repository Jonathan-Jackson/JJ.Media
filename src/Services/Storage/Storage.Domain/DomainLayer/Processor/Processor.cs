using Microsoft.Extensions.Logging;

namespace Storage.Domain.DomainLayer.Processor {

    public abstract class Processor {
        protected readonly ILogger<Processor> _logger;

        protected Processor(ILogger<Processor> logger) {
            _logger = logger;
        }
    }
}