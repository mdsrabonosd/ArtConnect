using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class CustomRequestRepository : Repository<CustomRequest>, ICustomRequestRepository
    {
        public CustomRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<CustomRequest>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<CustomRequest?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
