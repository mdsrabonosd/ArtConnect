using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IOrderItemFacade
    {
        Task<List<OrderItem>> GetOrderItemsAsync();
        Task<OrderItem?> GetOrderItemByIdAsync(int id);
        Task<OperationResult> AddOrderItemAsync(OrderItem item);
        Task<OperationResult> UpdateOrderItemAsync(OrderItem item);
        Task<OperationResult> DeleteOrderItemAsync(int id);
    }
}
