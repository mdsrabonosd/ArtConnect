using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class OrderFacade : IOrderFacade
    {
        private readonly IOrderRepository _repository;

        public OrderFacade(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddOrderAsync(Order item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Order created.");
        }

        public async Task<OperationResult> UpdateOrderAsync(Order item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Order updated.")
                : OperationResult.Fail("Order not found.");
        }

        public async Task<OperationResult> DeleteOrderAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Order deleted.")
                : OperationResult.Fail("Order not found.");
        }
    }
}
