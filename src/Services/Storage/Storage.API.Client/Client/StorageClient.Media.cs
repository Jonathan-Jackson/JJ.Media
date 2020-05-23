using JJ.Framework.Client;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Storage.API.Client.Client {

    public partial class StorageClient : ApiClient {

        public Task<HttpResponseMessage> ProcessTvShow(string filePath)
            => Post($"/api/media/process-tvshow", filePath);

        public Task<HttpResponseMessage> ProcessAnime(string filePath)
            => Post($"/api/media/process-anime", filePath);
    }
}