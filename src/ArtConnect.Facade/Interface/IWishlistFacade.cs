using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IWishlistFacade
    {
        Task<List<Wishlist>> GetWishlistsAsync();
        Task<Wishlist?> GetWishlistByIdAsync(int id);
        Task<OperationResult> AddWishlistAsync(Wishlist item);
        Task<OperationResult> UpdateWishlistAsync(Wishlist item);
        Task<OperationResult> DeleteWishlistAsync(int id);
    }
}
