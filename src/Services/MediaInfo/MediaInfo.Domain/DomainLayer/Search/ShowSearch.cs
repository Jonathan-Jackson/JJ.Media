using JJ.Media.MediaInfo.Services;
using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Exceptions;
using MediaInfo.Domain.Helpers.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaInfo.Domain.DomainLayer.Search {

    /// <summary>
    /// Searches for Shows via criteria.
    /// </summary>
    public class ShowSearch {
        private readonly IShowApiSearch _apiSearch;
        private readonly IShowRepository _showRepository;
        private readonly ShowStorage _showStorage;

        public ShowSearch(IShowApiSearch apiSearch, IShowRepository showRepository, ShowStorage showStorage) {
            _apiSearch = apiSearch;
            _showRepository = showRepository;
            _showStorage = showStorage;
        }

        /// <summary>
        /// Returns the closest matching show associated with any of the name values (by title).
        /// </summary>
        /// <param name="name">Title of the show.</param>
        /// <exception cref="SearchNotFoundException">
        /// Thrown if no value is found from the search.
        /// </exception>
        public Task<Show> SearchAsync(string name)
            => SearchAsync(new[] { name });

        /// <summary>
        /// Returns the closest matching show associated with any of the name values (by title).
        /// </summary>
        /// <param name="names">Titles of the show, should be ordered by priority.</param>
        /// <exception cref="SearchNotFoundException">
        /// Thrown if no value is found from the search.
        /// </exception>
        public async Task<Show> SearchAsync(IEnumerable<string> names) {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            if (!names.Any())
                throw new ArgumentException(nameof(names));

            // DB.
            var dbShows = await _showRepository.FindAsync(names);
            var foundShow = FindExactShow(names, dbShows);
            if (foundShow != null)
                return foundShow;

            // API.
            var apiShows = await _apiSearch.FindShowAsync(names);
            foundShow = FindExactShow(names, apiShows);
            if (foundShow != null) {
                foundShow.Id = await _showRepository.InsertAsync(foundShow);
                return foundShow;
            }

            // DB + API 'close' match.
            foundShow = FindSimilarShow(names, dbShows.Concat(apiShows));
            if (foundShow != null)
                return foundShow;
            else
                throw new SearchNotFoundException(string.Join(", ", names));
        }

        /// <summary>
        /// Returns the latest storage season.
        /// </summary>
        public int GetLatestStorageSeason(Show show, int episodeNumber) {
            var showFolder = _showStorage.FindShowFolder(show.GetPrimaryTitle());
            // TODO: how do we deal with episodes that are already there..?
            return showFolder?.Seasons?.Max(x => x.Season) ?? 1;
        }

        /// <summary>
        /// Returns a show which has an close title match (Case Insensitive).
        /// This primarily involves removing special characters and spaces to match.
        /// </summary>
        private Show? FindSimilarShow(IEnumerable<string> names, IEnumerable<Show> shows) {
            if (!names.Any() || !shows.Any())
                return null;

            foreach (var name in names.Select(GetLetters)) {
                var titleMatches = shows.Where(show => HasTitle(show, name));

                if (titleMatches.Any())
                    return titleMatches.OrderByDescending(show => HasPrimaryTitle(show, name)).First();
            }

            return null;
        }

        /// <summary>
        /// Returns a show which has an exacting title match (Case Insensitive).
        /// </summary>
        private Show? FindExactShow(IEnumerable<string> names, IEnumerable<Show> shows) {
            if (!names.Any() || !shows.Any())
                return null;

            foreach (var name in names) {
                var titleMatches = shows.Where(show => HasTitle(show, name));

                if (titleMatches.Any())
                    return titleMatches.OrderByDescending(show => HasPrimaryTitle(show, name)).First();
            }

            return null;
        }

        /// <summary>
        /// Returns a new string with only the letters found within the argument string.
        /// </summary>
        private string GetLetters(string arg)
            => new string(arg.Where(char.IsLetter).ToArray());

        /// <summary>
        /// Returns true if the show has the primary title.
        /// </summary>
        private bool HasPrimaryTitle(Show show, string title)
            => string.Equals(show.GetPrimaryTitle(), title, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns true if the show has the title.
        /// </summary>
        private bool HasTitle(Show show, string title)
            => show.Titles.Any(showTitle => string.Equals(showTitle.Title, title, StringComparison.OrdinalIgnoreCase));
    }
}