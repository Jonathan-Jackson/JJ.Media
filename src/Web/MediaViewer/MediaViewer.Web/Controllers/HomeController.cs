using MediaViewer.Web.Infrastructure;
using MediaViewer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IO = System.IO;

namespace MediaViewer.Web.Controllers {

    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly StorageApi _storageApi;

        public HomeController(ILogger<HomeController> logger, StorageApi storageApi) {
            _storageApi = storageApi;
            _logger = logger;
        }

        public IActionResult Index()
            => View("Index", new IndexViewModel());

        [HttpGet("watch-episode/{episodeGuid}")]
        public async Task<IActionResult> Index(Guid episodeGuid) {
            string path = await _storageApi.FindEpisodePath(episodeGuid);

            string webmPath = $"/{path.Substring(0, path.Length - 4)}.webm";
            bool isReady = IO.File.Exists(webmPath);

            return View("Index", new IndexViewModel {
                Path = webmPath,
                IsReady = isReady
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}