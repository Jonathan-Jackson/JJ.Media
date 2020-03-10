using JJ.Media.MediaInfo.Core.Entities;
using JJ.Media.MediaInfo.Data.Interfaces;
using JJ.Media.MediaInfo.Data.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Data.Repositories {

    public class Repository<TEntity> : SqlKataBase, IRepository<TEntity> where TEntity : Entity {
        private readonly string _tableName;

        public Repository(string tableName, IDbConnectionFactory dbFactory, Compiler sqlCompiler) : base(dbFactory, sqlCompiler) {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        public async Task DeleteAsync(TEntity entity)
            => await DeleteAsync(new[] { entity });

        public async Task DeleteAsync(IEnumerable<TEntity> entities) {
            Func<DisposableQuery, Task> query = async (DisposableQuery db) => {
                await db.Query(_tableName)
                    .Where("Id", entities)
                    .DeleteAsync();
            };

            await Execute(query);
        }

        public async Task<TEntity> FindAsync(int id)
            => (await FindAsync(new[] { id })).First();

        public async Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids) {
            Func<DisposableQuery, Task<IEnumerable<TEntity>>> query = async (DisposableQuery db)
                => await db.Query(_tableName)
                    .Where("Id", ids)
                    .GetAsync<TEntity>();

            return await Execute(query);
        }

        public async Task InsertAsync(TEntity entity) {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(TEntity entity) {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        private async Task<TResult> Execute<TResult>(Func<DisposableQuery, Task<TResult>> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectToDb()) {
                return await dbAction(db);
            }
        }

        private async Task Execute(Func<DisposableQuery, Task> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectToDb()) {
                await dbAction(db);
            }
        }
    }
}