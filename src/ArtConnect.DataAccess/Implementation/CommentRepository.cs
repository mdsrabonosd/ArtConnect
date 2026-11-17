using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Comment>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Artwork)
                .Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Artwork)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Comment>> GetByArtworkAsync(int artworkId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.ArtworkId == artworkId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
    }
}
