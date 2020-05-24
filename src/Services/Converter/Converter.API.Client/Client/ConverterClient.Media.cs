using JJ.Framework.Client;
using System.Threading.Tasks;

namespace Converter.API.Client.Client {

    public partial class ConverterClient : ApiClient {

        public Task VideoToWebm(string filePath)
            => Post("/api/media/video-to-webm-await", filePath);

        public Task EpisodeToWebm(string filePath, int episodeId, bool burnSubtitles)
            => Post("/api/media/episode-to-webm", new { FilePath = filePath, EpisodeId = episodeId, BurnSubtitles = burnSubtitles });

        public Task VideoToWebmWithWait(string filePath)
            => Post("/api/media/video-to-webm", filePath);
    }
}