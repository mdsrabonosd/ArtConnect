using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IArtistProposalRepository : IRepository<ArtistProposal>
    {
        Task<List<ArtistProposal>> GetAllWithDetailsAsync();
        Task<ArtistProposal?> GetByIdWithDetailsAsync(int id);

        Task<List<ArtistProposal>> GetByRequestAsync(int requestId);
        Task<List<ArtistProposal>> GetByArtistAsync(int artistProfileId);
    }
}
