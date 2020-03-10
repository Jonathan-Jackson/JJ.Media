using JJ.Media.MediaInfo.Data.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace JJ.Media.MediaInfo.Data.Factories {

    public class SqlConnectionFactory : IDbConnectionFactory {
        private string _connString;

        public SqlConnectionFactory(string connString) {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public IDbConnection Create()
            => new SqlConnection(_connString);
    }
}