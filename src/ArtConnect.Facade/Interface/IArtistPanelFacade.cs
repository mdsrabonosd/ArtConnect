using ArtConnect.Entities;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    /// <summary>Everything a signed-in artist can do. Every method checks that the data belongs to the artist.</summary>
    public interface IArtistPanelFacade
    {
        Task<ArtistProfile?> GetProfileAsync(int userId);
        Task<ArtistDashboard?> GetDashboardAsync(int userId);
        Task<OperationResult> UpdateProfileAsync(int userId, ArtistProfile form);

        Task<List<Artwork>> GetMyArtworksAsync(int userId);
        Task<Artwork?> GetMyArtworkAsync(int userId, int artworkId);
        Task<OperationResult> AddArtworkAsync(int userId, Artwork artwork, string? imageUrl);
        Task<OperationResult> UpdateArtworkAsync(int userId, Artwork artwork, string? newImageUrl);
        Task<OperationResult> DeleteArtworkAsync(int userId, int artworkId);

        Task<List<CustomRequest>> GetOpenRequestsAsync(int? categoryId);
        Task<CustomRequest?> GetOpenRequestAsync(int requestId);
        Task<ArtistProposal?> GetMyProposalForRequestAsync(int userId, int requestId);
        Task<OperationResult> SendProposalAsync(int userId, ArtistProposal proposal);
        Task<List<ArtistProposal>> GetMyProposalsAsync(int userId);
        Task<OperationResult> WithdrawProposalAsync(int userId, int proposalId);

        Task<List<Order>> GetMyOrdersAsync(int userId);
        Task<Order?> GetMyOrderAsync(int userId, int orderId);
        Task<OperationResult> StartOrderAsync(int userId, int orderId);
        Task<OperationResult> AddProgressPhotoAsync(int userId, int orderId, string imageUrl, string? note);
        Task<OperationResult> DeliverOrderAsync(int userId, int orderId);
    }
}
