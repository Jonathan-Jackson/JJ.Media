using System.Data;

namespace JJ.Media.Core.Infrastructure {

    /// <summary>
    /// Wrapper for an SQL connection to be created.
    /// </summary>
    public interface IDbConnectionFactory {

        IDbConnection Create();
    }
}