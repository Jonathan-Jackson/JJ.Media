namespace Storage.Domain.Helpers.Abstraction {

    public interface IShowStore : IStore {

        bool HasShowFolder(string primaryTitle);
    }
}