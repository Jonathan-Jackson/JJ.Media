using System.Data;

namespace JJ.Framework.Repository.Abstraction {

    /// <summary>
    /// Wrapper for an SQL connection to be created.
    /// </summary>
    public interface IDbConnectionFactory {

        IDbConnection Create();
    }
}