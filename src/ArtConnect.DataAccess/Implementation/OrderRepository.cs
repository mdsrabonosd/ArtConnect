using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
