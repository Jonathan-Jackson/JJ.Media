using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;

namespace JJ.Media.MediaInfo.Data.Models {
#pragma warning disable CA1063 // Implement IDisposable Correctly

    public class DisposableQuery : QueryFactory, IDisposable {
#pragma warning restore CA1063 // Implement IDisposable Correctly
        private readonly IDbConnection _connection;

        public DisposableQuery(IDbConnection connection, Compiler compiler)
            : base(connection, compiler) {
            _connection = connection;
        }

        public void Dispose() {
            _connection?.Dispose();
        }
    }
}