using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ArtistPanelFacade : IArtistPanelFacade
    {
        private const int MaxProgressPhotos = 10;
        private const string NoProfileMessage = "Your artist profile was not found. Please contact the administrator.";

        private readonly IArtistProfileRepository _profileRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICustomRequestRepository _requestRepository;
        private readonly IArtistProposalRepository _proposalRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProgressPhotoRepository _photoRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IArtistProfileFacade _profileFacade;
        private readonly IArtworkFacade _artworkFacade;
        private readonly IArtistProposalFacade _proposalFacade;

        public ArtistPanelFacade(
            IArtistProfileRepository profileRepository,
            IArtworkRepository artworkRepository,
            ICustomRequestRepository requestRepository,
            IArtistProposalRepository proposalRepository,
            IOrderRepository orderRepository,
            IProgressPhotoRepository photoRepository,
            IReviewRepository reviewRepository,
            IArtistProfileFacade profileFacade,
            IArtworkFacade artworkFacade,
            IArtistProposalFacade proposalFacade)
        {
            _profileRepository = profileRepository;
            _artworkRepository = artworkRepository;
            _requestRepository = requestRepository;
            _proposalRepository = proposalRepository;
            _orderRepository = orderRepository;
            _photoRepository = photoRepository;
            _reviewRepository = reviewRepository;
            _profileFacade = profileFacade;
            _artworkFacade = artworkFacade;
            _proposalFacade = proposalFacade;
        }

        // ---------------- profile and dashboard ----------------

        public async Task<ArtistProfile?> GetProfileAsync(int userId)
        {
            return await _profileRepository.GetByUserIdAsync(userId);
        }

        public async Task<ArtistDashboard?> GetDashboardAsync(int userId)
        {
            var basic = await _profileRepository.GetByUserIdAsync(userId);
            if (basic == null)
                return null;

            var profile = await _profileRepository.GetByIdWithDetailsAsync(basic.Id);
            if (profile == null)
                return null;

            var reviews = await _reviewRepository.GetByArtistAsync(profile.Id);

            return new ArtistDashboard
            {
                Profile = profile,
                TotalArtworks = await _artworkRepository.CountAsync(a => a.ArtistProfileId == profile.Id),
                SoldArtworks = await _artworkRepository.CountAsync(a => a.ArtistProfileId == profile.Id && a.Status == ArtworkStatus.Sold),
                PendingProposals = await _proposalRepository.CountAsync(p => p.ArtistProfileId == profile.Id && p.Status == ProposalStatus.Pending),
                ActiveOrders = await _orderRepository.CountAsync(o =>
                    o.ArtistProfileId == profile.Id &&
                    (o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress)),
                ReviewCount = reviews.Count,
                AverageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating),
                LatestReviews = reviews.Take(5).ToList()
            };
        }

        public async Task<OperationResult> UpdateProfileAsync(int userId, ArtistProfile form)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return OperationResult.Fail(NoProfileMessage);

            form.Id = profile.Id;
            var result = await _profileFacade.UpdateArtistProfileAsync(form);
            return result.Success ? OperationResult.Ok("Your artist profile was updated.") : result;
        }

        // ---------------- my artworks ----------------

        public async Task<List<Artwork>> GetMyArtworksAsync(int userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return new List<Artwork>();

            var all = await _artworkRepository.GetAllWithDetailsAsync();
            return all.Where(a => a.ArtistProfileId == profile.Id).ToList();
        }

        public async Task<Artwork?> GetMyArtworkAsync(int userId, int artworkId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            var artwork = await _artworkRepository.GetByIdWithDetailsAsync(artworkId);
            return artwork != null && artwork.ArtistProfileId == profile.Id ? artwork : null;
        }

        public async Task<OperationResult> AddArtworkAsync(int userId, Artwork artwork, string? imageUrl)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return OperationResult.Fail(NoProfileMessage);

            artwork.ArtistProfileId = profile.Id;
            return await _artworkFacade.AddArtworkAsync(artwork, imageUrl);
        }

        public async Task<OperationResult> UpdateArtworkAsync(int userId, Artwork artwork, string? newImageUrl)
        {
            var existing = await GetMyArtworkAsync(userId, artwork.Id);
            if (existing == null)
                return OperationResult.Fail("Artwork not found.");

            artwork.ArtistProfileId = existing.ArtistProfileId;
            return await _artworkFacade.UpdateArtworkAsync(artwork, newImageUrl);
        }

        public async Task<OperationResult> DeleteArtworkAsync(int userId, int artworkId)
        {
            var existing = await GetMyArtworkAsync(userId, artworkId);
            if (existing == null)
                return OperationResult.Fail("Artwork not found.");

            return await _artworkFacade.DeleteArtworkAsync(artworkId);
        }

        // ---------------- requests and proposals ----------------

        public async Task<List<CustomRequest>> GetOpenRequestsAsync(int? categoryId)
        {
            return await _requestRepository.GetOpenAsync(categoryId);
        }

        public async Task<CustomRequest?> GetOpenRequestAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            return request != null && request.Status == RequestStatus.Open ? request : null;
        }

        public async Task<ArtistProposal?> GetMyProposalForRequestAsync(int userId, int requestId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            var found = await _proposalRepository.FindAsync(p =>
                p.RequestId == requestId && p.ArtistProfileId == profile.Id);

            return found.FirstOrDefault();
        }

        public async Task<OperationResult> SendProposalAsync(int userId, ArtistProposal proposal)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return OperationResult.Fail(NoProfileMessage);

            proposal.ArtistProfileId = profile.Id;
            proposal.Status = ProposalStatus.Pending;

            var result = await _proposalFacade.AddArtistProposalAsync(proposal);
            return result.Success ? OperationResult.Ok("Your proposal was sent to the customer.") : result;
        }

        public async Task<List<ArtistProposal>> GetMyProposalsAsync(int userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return new List<ArtistProposal>();

            return await _proposalRepository.GetByArtistAsync(profile.Id);
        }

        public async Task<OperationResult> WithdrawProposalAsync(int userId, int proposalId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return OperationResult.Fail(NoProfileMessage);

            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null || proposal.ArtistProfileId != profile.Id)
                return OperationResult.Fail("Proposal not found.");

            if (proposal.Status != ProposalStatus.Pending)
                return OperationResult.Fail("Only a pending proposal can be withdrawn.");

            proposal.Status = ProposalStatus.Withdrawn;
            await _proposalRepository.UpdateAsync(proposal);
            return OperationResult.Ok("Your proposal was withdrawn.");
        }

        // ---------------- orders ----------------

        public async Task<List<Order>> GetMyOrdersAsync(int userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return new List<Order>();

            return await _orderRepository.GetByArtistAsync(profile.Id);
        }

        public async Task<Order?> GetMyOrderAsync(int userId, int orderId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            return order != null && order.ArtistProfileId == profile.Id ? order : null;
        }

        public async Task<OperationResult> StartOrderAsync(int userId, int orderId)
        {
            var order = await GetOwnedOrderAsync(userId, orderId);
            if (order == null)
                return OperationResult.Fail("Order not found.");

            if (order.Status != OrderStatus.Pending)
                return OperationResult.Fail("Only a pending order can be started.");

            order.Status = OrderStatus.InProgress;
            await _orderRepository.UpdateAsync(order);
            return OperationResult.Ok("The order is now in progress.");
        }

        public async Task<OperationResult> AddProgressPhotoAsync(int userId, int orderId, string imageUrl, string? note)
        {
            var order = await GetOwnedOrderAsync(userId, orderId);
            if (order == null)
                return OperationResult.Fail("Order not found.");

            if (order.Status != OrderStatus.InProgress)
                return OperationResult.Fail("Start the order before you upload progress photos.");

            if (await _photoRepository.CountAsync(p => p.OrderId == orderId) >= MaxProgressPhotos)
                return OperationResult.Fail($"You can upload at most {MaxProgressPhotos} progress photos for one order.");

            await _photoRepository.AddAsync(new ProgressPhoto
            {
                OrderId = orderId,
                ImageUrl = imageUrl,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            });

            return OperationResult.Ok("Progress photo uploaded. The customer can now see it.");
        }

        public async Task<OperationResult> DeliverOrderAsync(int userId, int orderId)
        {
            var order = await GetOwnedOrderAsync(userId, orderId);
            if (order == null)
                return OperationResult.Fail("Order not found.");

            if (order.Status != OrderStatus.InProgress)
                return OperationResult.Fail("Only an order that is in progress can be delivered.");

            if (order.OrderType == OrderType.Commission &&
                await _photoRepository.CountAsync(p => p.OrderId == orderId) == 0)
            {
                return OperationResult.Fail("Upload at least one progress photo before you mark a custom painting as delivered.");
            }

            order.Status = OrderStatus.Delivered;
            await _orderRepository.UpdateAsync(order);
            return OperationResult.Ok("The order was marked as delivered. The customer can now leave a review.");
        }

        private async Task<Order?> GetOwnedOrderAsync(int userId, int orderId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            var order = await _orderRepository.GetByIdAsync(orderId);
            return order != null && order.ArtistProfileId == profile.Id ? order : null;
        }
    }
}
