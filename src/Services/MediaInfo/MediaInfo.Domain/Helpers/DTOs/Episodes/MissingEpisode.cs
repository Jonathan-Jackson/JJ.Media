namespace MediaInfo.Domain.Helpers.DTOs.Episodes {

    public class MissingEpisode : Episode {

        public MissingEpisode(int showId, int season, int number) {
            ShowId = showId;
            SeasonNumber = season;
            EpisodeNumber = number;
        }
    }
}