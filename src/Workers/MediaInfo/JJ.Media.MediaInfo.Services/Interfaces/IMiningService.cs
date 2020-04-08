using JJ.Media.MediaInfo.Core.Models;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IMiningService {

        MinedEpisode MineEpisodeName(string name);
    }
}