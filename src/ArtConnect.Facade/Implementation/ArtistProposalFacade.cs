using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ArtistProposalFacade : IArtistProposalFacade
    {
        private readonly IArtistProposalRepository _repository;

        public ArtistProposalFacade(IArtistProposalRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ArtistProposal>> GetArtistProposalsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ArtistProposal?> GetArtistProposalByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddArtistProposalAsync(ArtistProposal item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Artist proposal created.");
        }

        public async Task<OperationResult> UpdateArtistProposalAsync(ArtistProposal item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Artist proposal updated.")
                : OperationResult.Fail("Artist proposal not found.");
        }

        public async Task<OperationResult> DeleteArtistProposalAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Artist proposal deleted.")
                : OperationResult.Fail("Artist proposal not found.");
        }
    }
}
