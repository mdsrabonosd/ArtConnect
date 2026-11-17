using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Order>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .Include(x => x.Items).ThenInclude(y => y.Artwork)
                .Include(x => x.ProgressPhotos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
