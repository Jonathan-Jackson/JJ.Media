using JJ.Media.Core.Entities;
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

        public async Task DeleteAsync(int id)
            => await DeleteAsync(new[] { id });

        public async Task DeleteAsync(IEnumerable<int> ids) {
            Func<DisposableQuery, Task> query = async (DisposableQuery db) => {
                await db.Query(_tableName)
                    .Where("Id", ids)
                    .DeleteAsync();
            };

            await Execute(query);
        }

        public async Task<TEntity> FindAsync(int id)
            => (await FindAsync(new[] { id })).First();

        public async Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids) {
            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("Id", ids)
                        .GetAsync<TEntity>()
            );
        }

        public async Task<int> InsertAsync(TEntity entity) {
            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).InsertAsync(entity)
            );
        }

        public async Task<IEnumerable<int>> InsertAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(TEntity entity) {
            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName)
                            .Where("Id", entity.Id)
                            .UpdateAsync(entity)
            );
        }

        public async Task<IEnumerable<int>> UpdateAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        #region Privates

        private async Task<TResult> Execute<TResult>(Func<DisposableQuery, Task<TResult>> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                return await dbAction(db);
            }
        }

        private async Task Execute(Func<DisposableQuery, Task> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                await dbAction(db);
            }
        }

        #endregion Privates
    }
}