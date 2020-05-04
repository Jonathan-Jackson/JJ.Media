﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace MediaInfo.Infrastructure.Helpers.Factories {

    /// <summary>
    /// Wrapper for an SQL connection to be created.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory {
        private string _connString;

        public SqlConnectionFactory(string connString) {
            _connString = string.IsNullOrWhiteSpace(connString)
                    ? throw new ArgumentException("Empty connection string.") : connString;
        }

        public IDbConnection Create()
            => new SqlConnection(_connString);
    }
}