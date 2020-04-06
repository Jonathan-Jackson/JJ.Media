using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JJ.Media.MediaInfo.Core.Entities.Episodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JJ.Media.MediaInfo.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase {

        // GET: api/Search/5
        [HttpGet("{showId}", Name = "Get")]
        public IEnumerable<Episode> Episodes(int showId) {
            return null;
        }

        // GET: api/Search/One%20Piece
        [HttpGet("{show}", Name = "Get")]
        public string Show(string show) {
            return null;
        }
    }
}