using ArtConnect.Entities;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    /// <summary>Everything a signed-in customer can do. Every method checks that the data belongs to the customer.</summary>
    public interface ICustomerFacade
    {
        Task<CustomerDashboard> GetDashboardAsync(int customerId);

        Task<List<CustomRequest>> GetMyRequestsAsync(int customerId);
        Task<CustomRequest?> GetMyRequestAsync(int requestId, int customerId);
        Task<List<ArtistProposal>> GetProposalsForRequestAsync(int requestId, int customerId);
        Task<OperationResult> CreateRequestAsync(CustomRequest request, int customerId);
        Task<OperationResult> CancelRequestAsync(int requestId, int customerId);
        Task<OperationResult> AcceptProposalAsync(int proposalId, int customerId);

        Task<List<Order>> GetMyOrdersAsync(int customerId);
        Task<Order?> GetMyOrderAsync(int orderId, int customerId);
        Task<OperationResult> AddReviewAsync(int orderId, int rating, string? comment, int customerId);

        Task<List<Wishlist>> GetWishlistAsync(int customerId);
        Task<OperationResult> RemoveFromWishlistAsync(int artworkId, int customerId);

        Task<List<Artwork>> GetCartArtworksAsync(IEnumerable<int> artworkIds);
        Task<OperationResult> CheckoutAsync(int customerId, IEnumerable<int> artworkIds);
    }
}
