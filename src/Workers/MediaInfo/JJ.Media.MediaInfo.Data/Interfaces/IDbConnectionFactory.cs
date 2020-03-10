using System.Data;

namespace JJ.Media.MediaInfo.Data.Interfaces {

    public interface IDbConnectionFactory {

        IDbConnection Create();
    }
}