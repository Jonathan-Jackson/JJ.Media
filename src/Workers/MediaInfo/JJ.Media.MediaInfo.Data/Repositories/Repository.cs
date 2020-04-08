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
        protected readonly string _tableName;

        public Repository(string tableName, IDbConnectionFactory dbFactory, Compiler sqlCompiler) : base(dbFactory, sqlCompiler) {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        public virtual async Task DeleteAsync(int id)
            => await DeleteAsync(new[] { id });

        public virtual async Task DeleteAsync(IEnumerable<int> ids) {
            Func<DisposableQuery, Task> query = async (DisposableQuery db) => {
                await db.Query(_tableName)
                    .Where("Id", ids)
                    .DeleteAsync();
            };

            await Execute(query);
        }

        public virtual async Task<TEntity> FindAsync(int id)
            => (await FindAsync(new[] { id })).First();

        public virtual async Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids) {
            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("Id", ids)
                        .GetAsync<TEntity>()
            );
        }

        public virtual async Task<int> InsertAsync(TEntity entity) {
            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).InsertAsync(entity)
            );
        }

        public virtual async Task<IEnumerable<int>> InsertAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        public virtual async Task<int> UpdateAsync(TEntity entity) {
            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName)
                            .Where("Id", entity.Id)
                            .UpdateAsync(entity)
            );
        }

        public virtual async Task<IEnumerable<int>> UpdateAsync(IEnumerable<TEntity> entities) {
            throw new NotImplementedException();
        }

        #region Privates

        protected virtual async Task<TResult> Execute<TResult>(Func<DisposableQuery, Task<TResult>> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                return await dbAction(db);
            }
        }

        protected virtual async Task Execute(Func<DisposableQuery, Task> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                await dbAction(db);
            }
        }

        #endregion Privates
    }
}