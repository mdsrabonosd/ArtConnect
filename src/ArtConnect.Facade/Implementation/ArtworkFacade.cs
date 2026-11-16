using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ArtworkFacade : IArtworkFacade
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArtistProfileRepository _profileRepository;

        public ArtworkFacade(
            IArtworkRepository artworkRepository,
            ICategoryRepository categoryRepository,
            IArtistProfileRepository profileRepository)
        {
            _artworkRepository = artworkRepository;
            _categoryRepository = categoryRepository;
            _profileRepository = profileRepository;
        }

        public async Task<List<Artwork>> GetArtworksAsync()
        {
            return await _artworkRepository.GetAllWithDetailsAsync();
        }

        public async Task<Artwork?> GetArtworkByIdAsync(int id)
        {
            return await _artworkRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddArtworkAsync(Artwork artwork, string? imageUrl)
        {
            ArgumentNullException.ThrowIfNull(artwork);

            var error = await ValidateAsync(artwork);
            if (error != null)
                return OperationResult.Fail(error);

            if (string.IsNullOrWhiteSpace(imageUrl))
                return OperationResult.Fail("At least one image is required.");

            artwork.Title = artwork.Title.Trim();
            artwork.Images.Add(new ArtworkImage { ImageUrl = imageUrl, IsMain = true });

            await _artworkRepository.AddAsync(artwork);
            return OperationResult.Ok("Artwork created.");
        }

        public async Task<OperationResult> UpdateArtworkAsync(Artwork artwork, string? newImageUrl)
        {
            ArgumentNullException.ThrowIfNull(artwork);

            var existing = await _artworkRepository.GetByIdWithDetailsAsync(artwork.Id);
            if (existing == null)
                return OperationResult.Fail("Artwork not found.");

            var error = await ValidateAsync(artwork);
            if (error != null)
                return OperationResult.Fail(error);

            existing.ArtistProfileId = artwork.ArtistProfileId;
            existing.CategoryId = artwork.CategoryId;
            existing.Title = artwork.Title.Trim();
            existing.Description = artwork.Description;
            existing.Medium = artwork.Medium;
            existing.Size = artwork.Size;
            existing.Price = artwork.Price;
            existing.IsOriginal = artwork.IsOriginal;
            existing.Status = artwork.Status;

            if (!string.IsNullOrWhiteSpace(newImageUrl))
            {
                foreach (var image in existing.Images)
                    image.IsMain = false;

                existing.Images.Add(new ArtworkImage { ImageUrl = newImageUrl, IsMain = true });
            }

            await _artworkRepository.UpdateAsync(existing);
            return OperationResult.Ok("Artwork updated.");
        }

        public async Task<OperationResult> DeleteArtworkAsync(int id)
        {
            var deleted = await _artworkRepository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Artwork deleted.")
                : OperationResult.Fail("Artwork not found.");
        }

        private async Task<string?> ValidateAsync(Artwork artwork)
        {
            if (string.IsNullOrWhiteSpace(artwork.Title))
                return "Title is required.";

            if (artwork.Price <= 0)
                return "Price must be greater than zero.";

            if (await _categoryRepository.GetByIdAsync(artwork.CategoryId) == null)
                return "Please select a valid category.";

            if (await _profileRepository.GetByIdAsync(artwork.ArtistProfileId) == null)
                return "Please select a valid artist.";

            return null;
        }
    }
}
