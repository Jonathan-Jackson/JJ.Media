using MediaInfo.API.Client.Models;
using System.IO;

namespace Storage.Domain.DomainLayer.Namer {

    public class EpisodeNamer {
        private const int MaxFileNameLength = 100;

        private readonly string _path;
        private readonly EpisodeSearchResult _search;

        public EpisodeNamer(string path, EpisodeSearchResult search) {
            _path = path;
            _search = search;
        }

        public string FileName
            => HasLargeName || string.IsNullOrWhiteSpace(_search.EpisodeTitle)
            ? $"{_search.ShowTitle} - S{SeasonNumber}E{EpisodeNumber}{FileExtension}"
            : $"{_search.ShowTitle} - S{SeasonNumber}E{EpisodeNumber} ({_search.EpisodeTitle}){FileExtension}";

        public string EpisodeNumber
            => _search.EpisodeNumber < 10 ? $"0{_search.EpisodeNumber}" : _search.EpisodeNumber.ToString();

        public string SeasonNumber
            => _search.SeasonNumber < 10 ? $"0{_search.SeasonNumber}" : _search.SeasonNumber.ToString();

        public string FileExtension
            => new FileInfo(_path).Extension;

        public string FolderPath
            // Default to Anime (since thats only whats supported atm).
            => Path.Combine("Anime", _search.ShowTitle, $"Season {_search.SeasonNumber}");

        private bool HasLargeName
            => (_search?.EpisodeTitle?.Length ?? 0 + _search.ShowTitle.Length) >= MaxFileNameLength;
    }
}