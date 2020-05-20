using JJ.Framework.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace Storage.API.Client.Client {

    public partial class StorageClient : ApiClient {

        public Task<HttpResponseMessage> Process(string filePath)
            => Post($"/api/media/process", filePath);
    }
}