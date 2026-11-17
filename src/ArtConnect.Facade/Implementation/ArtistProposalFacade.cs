using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ArtistProposalFacade : IArtistProposalFacade
    {
        private readonly IArtistProposalRepository _repository;
        private readonly ICustomRequestRepository _requestRepository;
        private readonly IArtistProfileRepository _artistRepository;
        private readonly IOrderRepository _orderRepository;

        public ArtistProposalFacade(
            IArtistProposalRepository repository,
            ICustomRequestRepository requestRepository,
            IArtistProfileRepository artistRepository,
            IOrderRepository orderRepository)
        {
            _repository = repository;
            _requestRepository = requestRepository;
            _artistRepository = artistRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<ArtistProposal>> GetArtistProposalsAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<ArtistProposal?> GetArtistProposalByIdAsync(int id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddArtistProposalAsync(ArtistProposal item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var error = await ValidateAsync(item, isNew: true);
            if (error != null)
                return OperationResult.Fail(error);


            await _repository.AddAsync(item);
            return OperationResult.Ok("Proposal created.");
        }

        public async Task<OperationResult> UpdateArtistProposalAsync(ArtistProposal item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var existing = await _repository.GetByIdAsync(item.Id);
            if (existing == null)
                return OperationResult.Fail("Proposal not found.");


            var error = await ValidateAsync(item, isNew: false);
            if (error != null)
                return OperationResult.Fail(error);

            existing.RequestId = item.RequestId;
            existing.ArtistProfileId = item.ArtistProfileId;
            existing.Price = item.Price;
            existing.DeliveryDays = item.DeliveryDays;
            existing.Message = item.Message;
            existing.RevisionsIncluded = item.RevisionsIncluded;
            existing.Status = item.Status;

            await _repository.UpdateAsync(existing);
            return OperationResult.Ok("Proposal updated.");
        }

        public async Task<OperationResult> DeleteArtistProposalAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Proposal not found.");

            if (await _orderRepository.AnyAsync(o => o.ProposalId == id))
                return OperationResult.Fail("An order was created from this proposal, so it cannot be deleted.");

            await _repository.DeleteAsync(id);
            return OperationResult.Ok("Proposal deleted.");
        }

        private async Task<string?> ValidateAsync(ArtistProposal item, bool isNew)
        {
            if (item.Price <= 0)
                return "Price must be greater than zero.";

            if (item.DeliveryDays < 1)
                return "Delivery days must be 1 or more.";

            if (item.RevisionsIncluded < 0 || item.RevisionsIncluded > 5)
                return "Revisions included must be between 0 and 5.";

            var message = (item.Message ?? string.Empty).Trim();
            if (message.Length < 10 || message.Length > 500)
                return "Message must be between 10 and 500 characters.";

            item.Message = message;

            var request = await _requestRepository.GetByIdAsync(item.RequestId);
            if (request == null)
                return "Please select a valid custom request.";

            var artist = await _artistRepository.GetByIdAsync(item.ArtistProfileId);
            if (artist == null)
                return "Please select a valid artist.";

            if (artist.UserId == request.CustomerId)
                return "An artist cannot send a proposal for his or her own request.";

            if (isNew && request.Status != RequestStatus.Open)
                return "Proposals can only be added to an Open request.";

            if (await _repository.AnyAsync(p => p.RequestId == item.RequestId
                    && p.ArtistProfileId == item.ArtistProfileId
                    && p.Id != item.Id))
                return "This artist already sent a proposal for this request.";

            return null;
        }
    }
}
