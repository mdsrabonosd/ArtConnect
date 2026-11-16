using ArtConnect.Entities;
using System.Linq.Expressions;

namespace ArtConnect.DataAccess.Interface
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);

        /// <summary>Soft delete: sets IsDeleted = true. The row stays in the database.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
