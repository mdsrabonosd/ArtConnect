using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {

        Task<List<Wishlist>> GetByUserAsync(int userId);
    }
}
