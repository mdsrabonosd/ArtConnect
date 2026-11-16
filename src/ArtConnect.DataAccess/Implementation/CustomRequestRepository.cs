using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class CustomRequestRepository : Repository<CustomRequest>, ICustomRequestRepository
    {
        public CustomRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
