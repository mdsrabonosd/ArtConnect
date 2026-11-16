using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class ArtworkImageRepository : Repository<ArtworkImage>, IArtworkImageRepository
    {
        public ArtworkImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
