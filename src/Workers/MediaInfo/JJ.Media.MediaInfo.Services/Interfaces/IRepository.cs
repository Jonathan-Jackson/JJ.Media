using JJ.Media.MediaInfo.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IRepository<TEntity> where TEntity : Entity {

        Task<TEntity> FindAsync(int id);

        Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids);

        Task InsertAsync(TEntity entity);

        Task InsertAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity);

        Task UpdateAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(IEnumerable<TEntity> entities);
    }
}