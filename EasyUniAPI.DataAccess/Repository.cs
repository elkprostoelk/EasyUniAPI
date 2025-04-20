using Microsoft.EntityFrameworkCore;

namespace EasyUniAPI.DataAccess
{
    public class Repository<TEntity, TIdentifier> : IRepository<TEntity, TIdentifier> where TEntity : class
    {
        private readonly EasyUniDbContext _dbContext;

        public DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        public Repository(EasyUniDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> InsertAsync(TEntity entity)
        {
            _dbContext.Add(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            _dbContext.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(TEntity entity)
        {
            _dbContext.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
