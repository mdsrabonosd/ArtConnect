using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class ArtistProposalRepository : Repository<ArtistProposal>, IArtistProposalRepository
    {
        public ArtistProposalRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
