using JJ.Framework.Client;
using System.Text.Json;
using System.Threading.Tasks;

namespace Converter.API.Client.Client {

    public partial class ConverterClient : ApiClient {

        public Task VideoToWebm(string filePath)
            => Post("/api/media/video-to-webm-await", filePath);

        public Task EpisodeToWebm(string filePath, int episodeId)
            => Post("/api/media/episode-to-webm", new { FilePath = filePath, EpisodeId = episodeId });

        public Task VideoToWebmWithWait(string filePath)
            => Post("/api/media/video-to-webm", filePath);
    }
}