using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class WishlistFacade : IWishlistFacade
    {
        private readonly IWishlistRepository _repository;

        public WishlistFacade(IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Wishlist>> GetWishlistsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Wishlist?> GetWishlistByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddWishlistAsync(Wishlist item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Wishlist created.");
        }

        public async Task<OperationResult> UpdateWishlistAsync(Wishlist item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Wishlist updated.")
                : OperationResult.Fail("Wishlist not found.");
        }

        public async Task<OperationResult> DeleteWishlistAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Wishlist deleted.")
                : OperationResult.Fail("Wishlist not found.");
        }
    }
}
