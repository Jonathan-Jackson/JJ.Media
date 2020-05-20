using Converter.IntegrationTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Coverter.IntegrationTesting.Controllers {

    public class MediaControllerTest : TestBase {
        private const string EscapedTestFile = @"TestFolder\\() t,e,s,t.mkv";
        private const string ExpectedFile = @"\\htpc\TestFolder\() t,e,s,t.webm";
        private const int ProcessWaitSeconds = 30;

        [Fact]
        public async Task Media_WebmSynchronous() {
            if (File.Exists(ExpectedFile))
                File.Delete(ExpectedFile);

            try {
                string body = JsonSerializer.Serialize(EscapedTestFile);
                var response = await _client.PostAsync($"/api/media/webm-synchronous", new StringContent(body, Encoding.UTF8, "application/json"));
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(File.Exists(ExpectedFile));
                Assert.NotEqual(0, new FileInfo(ExpectedFile).Length);

                // Short delay for file locks.
                await Task.Delay(5000);
            }
            finally {
                if (File.Exists(ExpectedFile))
                    File.Delete(ExpectedFile);
            }
        }

        [Fact]
        public async Task Media_Webm() {
            if (File.Exists(ExpectedFile))
                File.Delete(ExpectedFile);

            try {
                string body = JsonSerializer.Serialize(EscapedTestFile);
                var response = await _client.PostAsync($"/api/media/webm", new StringContent(body, Encoding.UTF8, "application/json"));
                Assert.True(HttpStatusCode.OK == response.StatusCode, $"Expected OK, Recieved {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");

                // Seek our result for 30seconds.
                for (int i = 0; i < ProcessWaitSeconds; i++)
                    if (File.Exists(ExpectedFile) && new FileInfo(ExpectedFile).Length > 0)
                        break;
                    else
                        await Task.Delay(1000);

                Assert.True(File.Exists(ExpectedFile));
                Assert.NotEqual(0, new FileInfo(ExpectedFile).Length);

                // Short delay for file locks.
                await Task.Delay(5000);
            }
            finally {
                if (File.Exists(ExpectedFile))
                    File.Delete(ExpectedFile);
            }
        }
    }
}