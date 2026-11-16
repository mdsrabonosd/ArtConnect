using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IArtworkFacade
    {
        Task<List<Artwork>> GetArtworksAsync();
        Task<Artwork?> GetArtworkByIdAsync(int id);

        /// <param name="imageUrl">URL of the main image that was already saved.</param>
        Task<OperationResult> AddArtworkAsync(Artwork artwork, string? imageUrl);

        /// <param name="newImageUrl">Optional new main image. Null keeps the current images.</param>
        Task<OperationResult> UpdateArtworkAsync(Artwork artwork, string? newImageUrl);

        Task<OperationResult> DeleteArtworkAsync(int id);
    }
}
