using System.Collections.Generic;
using System.Threading.Tasks;

namespace Converter.Core.Converters {

    public interface IConverter {

        Task Convert(string file, string outputDirectory, bool burnSubtitles);
    }
}