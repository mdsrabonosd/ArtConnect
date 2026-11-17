using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<List<Review>> GetAllWithDetailsAsync();
        Task<Review?> GetByIdWithDetailsAsync(int id);
    }
}
