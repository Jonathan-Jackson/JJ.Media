using DSharpPlus;
using System.Threading.Tasks;

namespace Discord.Core.Services {
    public interface IEpisodeAlertService {
        Task Alert(DiscordClient discord, int episodeId);
    }
}