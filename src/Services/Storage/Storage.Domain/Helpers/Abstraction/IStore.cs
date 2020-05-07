namespace Storage.Domain.Helpers.Abstraction {

    public interface IStore {

        string Save(string source, string folderPath, string fileName);
    }
}