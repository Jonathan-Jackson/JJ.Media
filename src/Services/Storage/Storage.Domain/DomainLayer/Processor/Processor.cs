using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Processor {

    public abstract class Processor {
        protected readonly ILogger<Processor> _logger;

        protected Processor(ILogger<Processor> logger) {
            _logger = logger;
        }

        public abstract Task ProcessAsync(string path);

        /// <summary>
        /// Gets the file name of an episode file path.
        /// </summary>
        protected string GetFileName(string episodePath)
            => new DirectoryInfo(episodePath)?.Name ?? string.Empty;
    }
}