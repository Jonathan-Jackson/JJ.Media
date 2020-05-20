using JJ.Framework.Repository.Abstraction;
using SqlKata.Compilers;
using System;
using System.Data;

namespace JJ.Framework.Repository {

    public class SqlKataBase {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly Compiler _compiler;

        protected SqlKataBase(IDbConnectionFactory dbFactory, Compiler compiler) {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        }

        protected DisposableQueryFactory ConnectQuery()
            => new DisposableQueryFactory(_dbFactory.Create(), _compiler);

        protected IDbConnection ConnectDb()
            => _dbFactory.Create();
    }
}