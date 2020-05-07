using System.IO;
using System.Threading.Tasks;

namespace Storage.Domain.DomainLayer.Processor {

    public abstract class Processor {

        public abstract Task ProcessAsync(string path);

        /// <summary>
        /// Gets the file name of an episode file path.
        /// </summary>
        protected string GetFileName(string episodePath)
            => new DirectoryInfo(episodePath)?.Name ?? string.Empty;
    }
}