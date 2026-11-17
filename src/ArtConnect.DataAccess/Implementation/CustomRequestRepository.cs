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

        public async Task<List<CustomRequest>> GetByCustomerAsync(int customerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Category)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<List<CustomRequest>> GetOpenAsync(int? categoryId)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Category)
                .Where(r => r.Status == RequestStatus.Open && !r.Customer.IsBlocked && !r.Customer.IsDeleted);

            if (categoryId.HasValue)
                query = query.Where(r => r.CategoryId == categoryId.Value);

            return await query.OrderByDescending(r => r.Id).ToListAsync();
        }
    }
}
