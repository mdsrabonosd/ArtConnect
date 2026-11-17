using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Review>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Order)
                .Include(x => x.Customer)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Order)
                .Include(x => x.Customer)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
