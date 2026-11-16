using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class ArtworkRepository : Repository<Artwork>, IArtworkRepository
    {
        public ArtworkRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Artwork>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.ArtistProfile).ThenInclude(p => p.User)
                .Include(a => a.Images)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<Artwork?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.ArtistProfile).ThenInclude(p => p.User)
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
