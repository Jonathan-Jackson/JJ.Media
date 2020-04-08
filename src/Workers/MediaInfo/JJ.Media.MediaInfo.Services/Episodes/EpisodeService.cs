using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Episodes {

    public class EpisodeService {
        private readonly IEpisodeRepository _episodeRepository;

        public EpisodeService(IEpisodeRepository episodeRepository) {
            _episodeRepository = episodeRepository ?? throw new ArgumentNullException(nameof(episodeRepository));
        }

        //public virtual Episode GetRecent() {
        //}

        public virtual async Task<Episode> Get(int id) {
            if (id <= 0)
                throw new ArgumentException(nameof(id));

            return await _episodeRepository.FindAsync(id)
                ?? throw new KeyNotFoundException();
        }

        public virtual async Task<Episode?> Find(int showId, uint seasonNumber, uint episodeNumber) {
            if (showId <= 0)
                throw new ArgumentException(nameof(showId));

            return await _episodeRepository.FindAsync(showId, seasonNumber, episodeNumber);
        }

        public virtual async Task<int> Add(Episode episode) {
            // add validator

            return await _episodeRepository.InsertAsync(episode);
        }

        public virtual async Task Delete(int id) {
            if (id <= 0)
                throw new ArgumentException(nameof(id));

            await _episodeRepository.DeleteAsync(id);
        }

        public virtual async Task Update(Episode episode) {
            // add validator

            await _episodeRepository.UpdateAsync(episode);
        }
    }
}