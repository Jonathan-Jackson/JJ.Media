using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;

namespace JJ.Media.Core.Infrastructure {

    /// <summary>
    /// Wrapper around a query factory to allow for disposing.
    /// </summary>
    public class DisposableQuery : QueryFactory, IDisposable {
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