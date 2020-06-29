using JJ.Framework.Controller;
using JJ.Framework.Repository;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJ.NET.Web.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase {
        private readonly MediaInfoClient _mediaInfoClient;

        public MovieController(MediaInfoClient mediaInfoClient) {
            _mediaInfoClient = mediaInfoClient;
        }

        // GET api/<EpisodeController>/5
        [HttpGet("paged")]
        public async Task<IActionResult> Paged(int index, int itemsPerPage) {
            if (index < 0)
                return BadRequest("Index cannot be negative.");
            if (itemsPerPage > 50)
                return BadRequest("Limited to 50 items per page.");

            var request = new PaginationRequest { Index = index, ItemsPerPage = itemsPerPage };
            //var result = await _mediaInfoClient.GetShowsPaginated(request);

            var items = Enumerable.Range(0, 14).Select(x => new { PrimaryTitle = "Lord Of The Rings", PosterUrl = "https://artworks.thetvdb.com/banners/movies/107/posters/2154812.jpg" }).ToArray();

            return Ok(
                new Pagination<dynamic> {
                    Index = 0,
                    ItemsPerPage = 15,
                    Items = items,
                    Total = 30
                });
        }
    }
}