using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IArtistProposalFacade
    {
        Task<List<ArtistProposal>> GetArtistProposalsAsync();
        Task<ArtistProposal?> GetArtistProposalByIdAsync(int id);
        Task<OperationResult> AddArtistProposalAsync(ArtistProposal item);
        Task<OperationResult> UpdateArtistProposalAsync(ArtistProposal item);
        Task<OperationResult> DeleteArtistProposalAsync(int id);
    }
}
