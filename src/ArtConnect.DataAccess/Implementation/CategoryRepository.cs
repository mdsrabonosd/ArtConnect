using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var normalized = name.Trim().ToLower();

            return await _dbSet.AnyAsync(c =>
                c.Name.ToLower() == normalized &&
                (excludeId == null || c.Id != excludeId));
        }
    }
}
