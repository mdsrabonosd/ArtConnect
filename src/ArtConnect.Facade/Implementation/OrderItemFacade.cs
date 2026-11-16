using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class OrderItemFacade : IOrderItemFacade
    {
        private readonly IOrderItemRepository _repository;

        public OrderItemFacade(IOrderItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddOrderItemAsync(OrderItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Order item created.");
        }

        public async Task<OperationResult> UpdateOrderItemAsync(OrderItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Order item updated.")
                : OperationResult.Fail("Order item not found.");
        }

        public async Task<OperationResult> DeleteOrderItemAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Order item deleted.")
                : OperationResult.Fail("Order item not found.");
        }
    }
}
