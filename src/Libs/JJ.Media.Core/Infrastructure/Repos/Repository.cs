using JJ.Media.Core.Entities;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.Core.Infrastructure {

    /// <summary>
    /// Base class for Repositories.
    /// </summary>
    public abstract class Repository<TEntity> : SqlKataBase, IRepository<TEntity> where TEntity : Entity {
        protected readonly string _tableName;

        public Repository(string tableName, IDbConnectionFactory dbFactory, Compiler sqlCompiler) : base(dbFactory, sqlCompiler) {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        /// <summary>
        /// Deletes an entity from the repository using its specified Id.
        /// </summary>
        public virtual async Task DeleteAsync(int id)
            => await DeleteAsync(new[] { id });

        /// <summary>
        /// Deletes a collection of entities from the repository using thier specified Ids.
        /// </summary>
        public virtual async Task DeleteAsync(IEnumerable<int> ids) {
            Func<DisposableQuery, Task> query = async (DisposableQuery db) => {
                await db.Query(_tableName)
                    .Where("Id", ids)
                    .DeleteAsync();
            };

            await Execute(query);
        }

        /// <summary>
        /// Finds an entity in the database using its specified Id.
        /// </summary>
        public virtual async Task<TEntity> FindAsync(int id)
            => (await FindAsync(new[] { id })).First();

        /// <summary>
        /// Finds a collection of entities in the database using their specified Ids.
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids) {
            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .WhereIn("Id", ids)
                        .GetAsync<TEntity>()
            );
        }

        public abstract Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Inserts entities in parallel.
        /// Consider overriding for performance (i.e. bulk insert).
        /// </summary>
        public async virtual Task<IEnumerable<int>> InsertAsync(IEnumerable<TEntity> entities) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any())
                return Enumerable.Empty<int>();

            return await Task.WhenAll(entities.Select(InsertAsync));
        }

        public abstract Task<int> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates entities in parallel.
        /// Consider overriding for performance (i.e. bulk insert).
        /// </summary>
        public async virtual Task<IEnumerable<int>> UpdateAsync(IEnumerable<TEntity> entities) {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any())
                return Enumerable.Empty<int>();

            return await Task.WhenAll(entities.Select(UpdateAsync));
        }

        #region Privates

        /// <summary>
        /// Executes a query against the repository,
        /// returning the expected result.
        /// </summary>
        protected virtual async Task<TResult> Execute<TResult>(Func<DisposableQuery, Task<TResult>> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                return await dbAction(db);
            }
        }

        /// <summary>
        /// Executes a query against the repository.
        /// </summary>
        protected virtual async Task Execute(Func<DisposableQuery, Task> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQuery db = ConnectQuery()) {
                await dbAction(db);
            }
        }

        #endregion Privates
    }
}