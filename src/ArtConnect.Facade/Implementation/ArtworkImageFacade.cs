using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ArtworkImageFacade : IArtworkImageFacade
    {
        private readonly IArtworkImageRepository _repository;

        public ArtworkImageFacade(IArtworkImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ArtworkImage>> GetArtworkImagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ArtworkImage?> GetArtworkImageByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddArtworkImageAsync(ArtworkImage item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Artwork image created.");
        }

        public async Task<OperationResult> UpdateArtworkImageAsync(ArtworkImage item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Artwork image updated.")
                : OperationResult.Fail("Artwork image not found.");
        }

        public async Task<OperationResult> DeleteArtworkImageAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Artwork image deleted.")
                : OperationResult.Fail("Artwork image not found.");
        }
    }
}
