using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IOrderFacade
    {
        Task<List<Order>> GetOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<OperationResult> AddOrderAsync(Order item);
        Task<OperationResult> UpdateOrderAsync(Order item);
        Task<OperationResult> DeleteOrderAsync(int id);
    }
}
