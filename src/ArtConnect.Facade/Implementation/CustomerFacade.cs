using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class CustomerFacade : ICustomerFacade
    {
        private const int MaxOpenRequests = 5;
        private const decimal CommissionRate = 0.10m;

        private readonly ICustomRequestRepository _requestRepository;
        private readonly IArtistProposalRepository _proposalRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICustomRequestFacade _requestFacade;
        private readonly IReviewFacade _reviewFacade;

        public CustomerFacade(
            ICustomRequestRepository requestRepository,
            IArtistProposalRepository proposalRepository,
            IOrderRepository orderRepository,
            IWishlistRepository wishlistRepository,
            IArtworkRepository artworkRepository,
            ICustomRequestFacade requestFacade,
            IReviewFacade reviewFacade)
        {
            _requestRepository = requestRepository;
            _proposalRepository = proposalRepository;
            _orderRepository = orderRepository;
            _wishlistRepository = wishlistRepository;
            _artworkRepository = artworkRepository;
            _requestFacade = requestFacade;
            _reviewFacade = reviewFacade;
        }

        public async Task<CustomerDashboard> GetDashboardAsync(int customerId)
        {
            var requests = await _requestRepository.GetByCustomerAsync(customerId);

            return new CustomerDashboard
            {
                OpenRequests = requests.Count(r => r.Status == RequestStatus.Open),
                PendingProposals = await _proposalRepository.CountAsync(p =>
                    p.CustomRequest.CustomerId == customerId && p.Status == ProposalStatus.Pending),
                ActiveOrders = await _orderRepository.CountAsync(o =>
                    o.CustomerId == customerId &&
                    (o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress)),
                WishlistCount = await _wishlistRepository.CountAsync(w => w.UserId == customerId),
                RecentRequests = requests.Take(5).ToList()
            };
        }

        // ---------------- custom requests ----------------

        public async Task<List<CustomRequest>> GetMyRequestsAsync(int customerId)
        {
            return await _requestRepository.GetByCustomerAsync(customerId);
        }

        public async Task<CustomRequest?> GetMyRequestAsync(int requestId, int customerId)
        {
            var request = await _requestRepository.GetByIdWithDetailsAsync(requestId);
            return request != null && request.CustomerId == customerId ? request : null;
        }

        public async Task<List<ArtistProposal>> GetProposalsForRequestAsync(int requestId, int customerId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.CustomerId != customerId)
                return new List<ArtistProposal>();

            return await _proposalRepository.GetByRequestAsync(requestId);
        }

        public async Task<OperationResult> CreateRequestAsync(CustomRequest request, int customerId)
        {
            ArgumentNullException.ThrowIfNull(request);

            var openCount = await _requestRepository.CountAsync(r =>
                r.CustomerId == customerId && r.Status == RequestStatus.Open);

            if (openCount >= MaxOpenRequests)
                return OperationResult.Fail($"You can have at most {MaxOpenRequests} open requests at the same time.");

            request.CustomerId = customerId;
            request.Status = RequestStatus.Open;

            var result = await _requestFacade.AddCustomRequestAsync(request);
            return result.Success ? OperationResult.Ok("Your request was published. Artists can now send proposals.") : result;
        }

        public async Task<OperationResult> CancelRequestAsync(int requestId, int customerId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.CustomerId != customerId)
                return OperationResult.Fail("Request not found.");

            if (request.Status != RequestStatus.Open)
                return OperationResult.Fail("Only an open request can be cancelled.");

            request.Status = RequestStatus.Cancelled;
            await _requestRepository.UpdateAsync(request);
            return OperationResult.Ok("Your request was cancelled.");
        }

        public async Task<OperationResult> AcceptProposalAsync(int proposalId, int customerId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null)
                return OperationResult.Fail("Proposal not found.");

            var request = await _requestRepository.GetByIdAsync(proposal.RequestId);
            if (request == null || request.CustomerId != customerId)
                return OperationResult.Fail("Proposal not found.");

            if (request.Status != RequestStatus.Open)
                return OperationResult.Fail("This request is no longer open.");

            if (proposal.Status != ProposalStatus.Pending)
                return OperationResult.Fail("This proposal can no longer be accepted.");

            // 1. Accept the chosen proposal.
            proposal.Status = ProposalStatus.Accepted;
            await _proposalRepository.UpdateAsync(proposal);

            // 2. Reject every other pending proposal of the same request.
            var others = await _proposalRepository.FindAsync(p =>
                p.RequestId == request.Id && p.Id != proposal.Id && p.Status == ProposalStatus.Pending);

            foreach (var other in others)
            {
                var tracked = await _proposalRepository.GetByIdAsync(other.Id);
                if (tracked == null) continue;

                tracked.Status = ProposalStatus.Rejected;
                await _proposalRepository.UpdateAsync(tracked);
            }

            // 3. Close the request.
            request.Status = RequestStatus.Accepted;
            await _requestRepository.UpdateAsync(request);

            // 4. Create the commission order (commission is simulated).
            await _orderRepository.AddAsync(new Order
            {
                CustomerId = customerId,
                ArtistProfileId = proposal.ArtistProfileId,
                ProposalId = proposal.Id,
                OrderType = OrderType.Commission,
                TotalAmount = proposal.Price,
                CommissionAmount = Math.Round(proposal.Price * CommissionRate, 2),
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            });

            return OperationResult.Ok("Proposal accepted. Your order was created.");
        }

        // ---------------- orders and reviews ----------------

        public async Task<List<Order>> GetMyOrdersAsync(int customerId)
        {
            return await _orderRepository.GetByCustomerAsync(customerId);
        }

        public async Task<Order?> GetMyOrderAsync(int orderId, int customerId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            return order != null && order.CustomerId == customerId ? order : null;
        }

        public async Task<OperationResult> AddReviewAsync(int orderId, int rating, string? comment, int customerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.CustomerId != customerId)
                return OperationResult.Fail("Order not found.");

            var result = await _reviewFacade.AddReviewAsync(new Review
            {
                OrderId = orderId,
                Rating = rating,
                Comment = comment
            });

            return result.Success ? OperationResult.Ok("Thank you! Your review was saved.") : result;
        }

        // ---------------- wishlist and cart ----------------

        public async Task<List<Wishlist>> GetWishlistAsync(int customerId)
        {
            return await _wishlistRepository.GetByUserAsync(customerId);
        }

        public async Task<OperationResult> RemoveFromWishlistAsync(int artworkId, int customerId)
        {
            var existing = (await _wishlistRepository.FindAsync(w =>
                w.ArtworkId == artworkId && w.UserId == customerId)).FirstOrDefault();

            if (existing == null)
                return OperationResult.Fail("This artwork is not in your wishlist.");

            await _wishlistRepository.DeleteAsync(existing.Id);
            return OperationResult.Ok("Removed from your wishlist.");
        }

        public async Task<List<Artwork>> GetCartArtworksAsync(IEnumerable<int> artworkIds)
        {
            var result = new List<Artwork>();
            foreach (var id in artworkIds.Distinct())
            {
                var artwork = await _artworkRepository.GetByIdWithDetailsAsync(id);
                if (artwork != null)
                    result.Add(artwork);
            }

            return result;
        }

        public async Task<OperationResult> CheckoutAsync(int customerId, IEnumerable<int> artworkIds)
        {
            var ids = artworkIds.Distinct().ToList();
            if (ids.Count == 0)
                return OperationResult.Fail("Your cart is empty.");

            var artworks = new List<Artwork>();
            foreach (var id in ids)
            {
                var artwork = await _artworkRepository.GetByIdWithDetailsAsync(id);

                if (artwork == null)
                    return OperationResult.Fail("An artwork in your cart is no longer available.");

                if (artwork.Status != ArtworkStatus.Available)
                    return OperationResult.Fail($"{artwork.Title} has already been sold. Please remove it from your cart.");

                if (artwork.ArtistProfile.UserId == customerId)
                    return OperationResult.Fail("You cannot buy your own artwork.");

                if (artwork.ArtistProfile.User.IsBlocked || artwork.ArtistProfile.User.IsDeleted)
                    return OperationResult.Fail($"{artwork.Title} is not available right now.");

                artworks.Add(artwork);
            }

            // One order for each artist. The artworks are tracked, so marking them as
            // Sold is saved in the same SaveChanges as the order.
            foreach (var group in artworks.GroupBy(a => a.ArtistProfileId))
            {
                var total = group.Sum(a => a.Price);

                var order = new Order
                {
                    CustomerId = customerId,
                    ArtistProfileId = group.Key,
                    OrderType = OrderType.Purchase,
                    TotalAmount = total,
                    CommissionAmount = Math.Round(total * CommissionRate, 2),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow
                };

                foreach (var artwork in group)
                {
                    order.Items.Add(new OrderItem { ArtworkId = artwork.Id, Price = artwork.Price });
                    artwork.Status = ArtworkStatus.Sold;
                }

                await _orderRepository.AddAsync(order);
            }

            return OperationResult.Ok("Your order was placed. Payment is simulated in this version.");
        }
    }
}
