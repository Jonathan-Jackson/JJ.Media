using Converter.API.Models;
using System.Threading.Tasks;

namespace Converter.API.Converter {

    public interface IMediaConverter {

        Task<string> Convert(ConvertSettings settings);
    }
}