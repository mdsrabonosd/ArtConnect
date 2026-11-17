using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IArtistProposalRepository : IRepository<ArtistProposal>
    {
        Task<List<ArtistProposal>> GetAllWithDetailsAsync();
        Task<ArtistProposal?> GetByIdWithDetailsAsync(int id);
    }
}
