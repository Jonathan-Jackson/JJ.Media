﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Framework.Repository.Abstraction {

    public interface IRepository<TEntity> where TEntity : Entity {

        public Task DeleteAsync(int id);

        public Task DeleteAsync(IEnumerable<int> ids);

        public Task<TEntity> FindAsync(int id);

        public Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids);

        public abstract Task<Pagination<TEntity>> FindPaginatedAsync(int count, int skip);

        public abstract Task<int> InsertAsync(TEntity entity);

        public abstract Task<IEnumerable<int>> InsertAsync(IEnumerable<TEntity> entities);

        public abstract Task<int> UpdateAsync(TEntity entity);

        public abstract Task<IEnumerable<int>> UpdateAsync(IEnumerable<TEntity> entities);
    }
}