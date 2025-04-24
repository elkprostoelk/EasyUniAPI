using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.DataAccess
{
    public interface IRepository<TEntity, TIdentifier> where TEntity : class
    {
        DbSet<TEntity> DbSet { get; }

        Task<bool> InsertAsync(TEntity entity);
        Task<bool> InsertRangeAsync(List<TEntity> entities);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> RemoveAsync(TEntity entity);
    }
}
