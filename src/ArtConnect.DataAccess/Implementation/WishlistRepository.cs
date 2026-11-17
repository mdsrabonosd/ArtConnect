using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Wishlist>> GetByUserAsync(int userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(w => w.Artwork).ThenInclude(a => a.Images)
                .Include(w => w.Artwork).ThenInclude(a => a.ArtistProfile).ThenInclude(p => p.User)
                .Include(w => w.Artwork).ThenInclude(a => a.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Id)
                .ToListAsync();
        }
    }
}
