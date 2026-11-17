using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ReviewFacade : IReviewFacade
    {
        private readonly IReviewRepository _repository;
        private readonly IOrderRepository _orderRepository;

        public ReviewFacade(
            IReviewRepository repository,
            IOrderRepository orderRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddReviewAsync(Review item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var error = await ValidateAsync(item, isNew: true);
            if (error != null)
                return OperationResult.Fail(error);

            // The customer and the artist always come from the order.
            var order = await _orderRepository.GetByIdAsync(item.OrderId);
            item.CustomerId = order!.CustomerId;
            item.ArtistProfileId = order.ArtistProfileId;

            await _repository.AddAsync(item);
            return OperationResult.Ok("Review created.");
        }

        public async Task<OperationResult> UpdateReviewAsync(Review item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var existing = await _repository.GetByIdAsync(item.Id);
            if (existing == null)
                return OperationResult.Fail("Review not found.");

            if (item.OrderId != existing.OrderId)
                return OperationResult.Fail("The order of a review cannot be changed.");

            var error = await ValidateAsync(item, isNew: false);
            if (error != null)
                return OperationResult.Fail(error);

            existing.Rating = item.Rating;
            existing.Comment = item.Comment;

            await _repository.UpdateAsync(existing);
            return OperationResult.Ok("Review updated.");
        }

        public async Task<OperationResult> DeleteReviewAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Review not found.");


            await _repository.DeleteAsync(id);
            return OperationResult.Ok("Review deleted.");
        }

        private async Task<string?> ValidateAsync(Review item, bool isNew)
        {
            if (item.Rating < 1 || item.Rating > 5)
                return "Rating must be between 1 and 5.";

            var order = await _orderRepository.GetByIdAsync(item.OrderId);
            if (order == null)
                return "Please select a valid order.";

            if (isNew)
            {
                if (order.Status != OrderStatus.Delivered)
                    return "A review can only be added to a delivered order.";

                if (await _repository.AnyAsync(r => r.OrderId == item.OrderId))
                    return "This order already has a review.";
            }

            return null;
        }
    }
}
