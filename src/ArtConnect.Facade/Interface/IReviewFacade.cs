using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IReviewFacade
    {
        Task<List<Review>> GetReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int id);
        Task<OperationResult> AddReviewAsync(Review item);
        Task<OperationResult> UpdateReviewAsync(Review item);
        Task<OperationResult> DeleteReviewAsync(int id);
    }
}
