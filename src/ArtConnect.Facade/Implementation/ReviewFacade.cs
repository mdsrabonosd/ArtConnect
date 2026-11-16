using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ReviewFacade : IReviewFacade
    {
        private readonly IReviewRepository _repository;

        public ReviewFacade(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddReviewAsync(Review item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Review created.");
        }

        public async Task<OperationResult> UpdateReviewAsync(Review item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Review updated.")
                : OperationResult.Fail("Review not found.");
        }

        public async Task<OperationResult> DeleteReviewAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Review deleted.")
                : OperationResult.Fail("Review not found.");
        }
    }
}
