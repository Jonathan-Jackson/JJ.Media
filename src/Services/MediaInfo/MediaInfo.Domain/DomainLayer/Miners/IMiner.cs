using MediaInfo.Domain.Helpers.DTOs.Miners;

namespace MediaInfo.DomainLayer.Miners {

    /// <summary>
    /// Rips information from a string.
    /// </summary>
    public interface IMiner {

        MinedEpisode MineEpisodeName(string name);
    }
}