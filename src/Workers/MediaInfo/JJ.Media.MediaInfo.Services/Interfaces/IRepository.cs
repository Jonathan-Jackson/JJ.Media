using JJ.Media.MediaInfo.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IRepository<TEntity> where TEntity : Entity {

        Task<TEntity> FindAsync(int id);

        Task<IEnumerable<TEntity>> FindAsync(IEnumerable<int> ids);

        Task<int> InsertAsync(TEntity entity);

        Task<IEnumerable<int>> InsertAsync(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(TEntity entity);

        Task<IEnumerable<int>> UpdateAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(int id);

        Task DeleteAsync(IEnumerable<int> id);
    }
}