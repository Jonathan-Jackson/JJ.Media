using System.Data;

namespace MediaInfo.Infrastructure.Helpers.Factories {

    /// <summary>
    /// Wrapper for an SQL connection to be created.
    /// </summary>
    public interface IDbConnectionFactory {

        IDbConnection Create();
    }
}