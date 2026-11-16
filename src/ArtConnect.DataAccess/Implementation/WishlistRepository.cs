using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
