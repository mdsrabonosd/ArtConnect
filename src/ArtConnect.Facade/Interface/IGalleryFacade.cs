using ArtConnect.Entities;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    /// <summary>Public gallery: search, artwork details, artist portfolios and social actions.</summary>
    public interface IGalleryFacade
    {
        Task<PagedResult<Artwork>> SearchAsync(ArtworkFilter filter);
        Task<List<Artwork>> GetLatestAsync(int count);
        Task<List<string>> GetMediumsAsync();
        Task<ArtworkDetailsModel?> GetDetailsAsync(int artworkId, int? userId);

        Task<List<ArtistProfile>> GetArtistsAsync();
        Task<ArtistPortfolioModel?> GetArtistPortfolioAsync(int artistProfileId);

        Task<OperationResult> ToggleLikeAsync(int artworkId, int userId);
        Task<OperationResult> ToggleWishlistAsync(int artworkId, int userId);
        Task<OperationResult> AddCommentAsync(int artworkId, int userId, string text);
        Task<OperationResult> DeleteCommentAsync(int commentId, int userId);
    }
}
