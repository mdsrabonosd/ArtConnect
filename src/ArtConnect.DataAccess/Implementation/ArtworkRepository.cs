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

        private IQueryable<Artwork> VisibleWithDetails()
        {
            return _dbSet
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.ArtistProfile).ThenInclude(p => p.User)
                .Include(a => a.Images)
                .Where(a => !a.ArtistProfile.User.IsBlocked && !a.ArtistProfile.User.IsDeleted);
        }

        public async Task<(List<Artwork> Items, int TotalCount)> SearchAsync(
            string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, string? medium, int page, int pageSize)
        {
            var query = VisibleWithDetails();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var text = search.Trim();
                query = query.Where(a =>
                    a.Title.Contains(text) ||
                    (a.Description != null && a.Description.Contains(text)) ||
                    a.ArtistProfile.User.FullName.Contains(text));
            }

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(a => a.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(a => a.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(medium))
                query = query.Where(a => a.Medium == medium);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<Artwork>> GetByArtistAsync(int artistProfileId)
        {
            return await VisibleWithDetails()
                .Where(a => a.ArtistProfileId == artistProfileId)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<List<Artwork>> GetLatestAsync(int count)
        {
            return await VisibleWithDetails()
                .Where(a => a.Status == ArtworkStatus.Available)
                .OrderByDescending(a => a.Id)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<string>> GetMediumsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(a => !a.ArtistProfile.User.IsBlocked && !a.ArtistProfile.User.IsDeleted)
                .Select(a => a.Medium)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
    }
}
