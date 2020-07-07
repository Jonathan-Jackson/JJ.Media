using JJ.Framework.Repository.Abstraction;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Framework.Repository {

    public abstract class Repository<TEntity> : SqlKataBase, IRepository<TEntity> where TEntity : Entity {
        protected readonly string _tableName;

        public Repository(string tableName, IDbConnectionFactory dbFactory, Compiler sqlCompiler) : base(dbFactory, sqlCompiler) {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        /// <summary>
        /// Returns the entity paginated.
        /// </summary>
        public virtual async Task<Pagination<TEntity>> FindPaginatedAsync(int count, int skip) {
            var pagination = new Pagination<TEntity> { ItemsPerPage = count, Index = skip / count };

            // update to single transaction.
            pagination.Items = (await Execute((DisposableQueryFactory db)
                    => db.Query(_tableName)
                        .Skip(skip)
                        .Limit(count)
                        .GetAsync<TEntity>()
            )).ToArray();

            pagination.Total = await Execute((DisposableQueryFactory db)
                                => db.Query(_tableName)
                                    .CountAsync<int>());

            return pagination;
        }

        /// <summary>
        /// Deletes an entity from the repository using its specified Id.
        /// </summary>
        public virtual Task DeleteAsync(int id)
            => DeleteAsync(new[] { id });

        /// <summary>
        /// Deletes a collection of entities from the repository using thier specified Ids.
        /// </summary>
        public virtual Task DeleteAsync(IEnumerable<int> ids) {
            Func<DisposableQueryFactory, Task> query = async (DisposableQueryFactory db) => {
                await db.Query(_tableName)
                    .WhereIn("Id", ids)
                    .DeleteAsync();
            };

            return Execute(query);
        }

        /// <summary>
        /// Finds an entity in the database using its specified Id.
        /// </summary>
        public virtual Task<TEntity> FindAsync(int id)
            => Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .Where("Id", id)
                        .FirstOrDefaultAsync<TEntity>());

        /// <summary>
        /// Finds a collection of entities in the database using their specified Ids.
        /// </summary>
        public virtual Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids)
            => Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .WhereIn("Id", ids)
                        .GetAsync<TEntity>());

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
        protected virtual async Task<TResult> Execute<TResult>(Func<DisposableQueryFactory, Task<TResult>> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQueryFactory db = ConnectQuery()) {
                return await dbAction(db);
            }
        }

        /// <summary>
        /// Executes a query against the repository.
        /// </summary>
        protected virtual async Task Execute(Func<DisposableQueryFactory, Task> dbAction) {
            // TODO: add retry for transient errors.
            using (DisposableQueryFactory db = ConnectQuery()) {
                await dbAction(db);
            }
        }

        #endregion Privates
    }
}