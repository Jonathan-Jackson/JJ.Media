﻿using JJ.Framework.Controller;
using JJ.Framework.Repository;
using MediaInfo.API.Client.Models;
using System.Threading.Tasks;

namespace MediaInfo.API.Client.Client {

    public partial class MediaInfoClient {

        public Task<Show> GetShow(int showId)
            => Get<Show>($"api/show/{showId}");

        public Task<Pagination<Show>> GetShowsPaginated(PaginationRequest request)
            => Post<Pagination<Show>>($"api/show/paginated", request);

        public Task<string[]> GetShowImages(int showId)
            => Get<string[]>($"api/show/{showId}/images");

        public Task<string> GetShowOverview(int showId)
            => Get<string>($"api/show/{showId}/overview");

        public Task<string> GetShowExternalLink(int showId)
            => Get<string>($"api/show/{showId}/api/showlink");
    }
}