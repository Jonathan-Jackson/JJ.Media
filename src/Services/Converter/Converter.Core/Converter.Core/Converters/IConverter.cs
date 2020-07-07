using System.Collections.Generic;
using System.Threading.Tasks;

namespace Converter.Core.Converters {

    public interface IConverter {

        Task Convert(IReadOnlyCollection<string> files, bool burnSubtitles);

        Task Convert(string file, bool burnSubtitles);
    }
}