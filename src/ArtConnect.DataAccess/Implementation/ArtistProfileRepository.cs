using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class ArtistProfileRepository : Repository<ArtistProfile>, IArtistProfileRepository
    {
        public ArtistProfileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ArtistProfile>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(a => a.User)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<ArtistProfile?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ArtistProfile?> GetByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
