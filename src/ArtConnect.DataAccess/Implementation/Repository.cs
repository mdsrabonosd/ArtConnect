using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ArtConnect.DataAccess.Implementation
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        // Soft-deleted rows are removed by the global query filter in ApplicationDbContext.
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (existing == null)
            {
                return false;
            }

            // Keep the system fields, copy everything else from the posted entity.
            var createdAt = existing.CreatedAt;
            var isDeleted = existing.IsDeleted;

            _dbContext.Entry(existing).CurrentValues.SetValues(entity);

            existing.CreatedAt = createdAt;
            existing.IsDeleted = isDeleted;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
