using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IArtworkImageFacade
    {
        Task<List<ArtworkImage>> GetArtworkImagesAsync();
        Task<ArtworkImage?> GetArtworkImageByIdAsync(int id);
        Task<OperationResult> AddArtworkImageAsync(ArtworkImage item);
        Task<OperationResult> UpdateArtworkImageAsync(ArtworkImage item);
        Task<OperationResult> DeleteArtworkImageAsync(int id);
    }
}
