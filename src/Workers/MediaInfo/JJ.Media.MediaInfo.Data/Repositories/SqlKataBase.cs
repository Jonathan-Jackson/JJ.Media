using JJ.Media.MediaInfo.Data.Interfaces;
using JJ.Media.MediaInfo.Data.Models;
using SqlKata.Compilers;
using System;
using System.Data;

namespace JJ.Media.MediaInfo.Data.Repositories {

    public abstract class SqlKataBase {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly Compiler _compiler;

        protected SqlKataBase(IDbConnectionFactory dbFactory, Compiler compiler) {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        }

        protected DisposableQuery ConnectQuery()
            => new DisposableQuery(_dbFactory.Create(), _compiler);

        protected IDbConnection ConnectDb()
            => _dbFactory.Create();
    }
}