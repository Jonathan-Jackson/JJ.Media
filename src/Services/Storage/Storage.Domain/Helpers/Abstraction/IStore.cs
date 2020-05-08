namespace Storage.Domain.Helpers.Abstraction {

    public interface IStore {

        string SaveDownload(string source, string folderPath, string fileName);
    }
}