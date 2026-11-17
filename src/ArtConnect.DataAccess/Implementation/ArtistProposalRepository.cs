using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess.Implementation
{
    public class ArtistProposalRepository : Repository<ArtistProposal>, IArtistProposalRepository
    {
        public ArtistProposalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ArtistProposal>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.CustomRequest)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<ArtistProposal?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(x => x.CustomRequest)
                .Include(x => x.ArtistProfile).ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
