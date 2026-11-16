using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Implementation
{
    public class ProgressPhotoRepository : Repository<ProgressPhoto>, IProgressPhotoRepository
    {
        public ProgressPhotoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
