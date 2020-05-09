using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Abstraction {

    public interface IEpisodeStore : IStore {

        Task<string> SaveDownload(string source, string folderPath, string fileName);
    }
}