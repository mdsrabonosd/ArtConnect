using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
