using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetAllWithDetailsAsync();
        Task<Order?> GetByIdWithDetailsAsync(int id);

        Task<List<Order>> GetByCustomerAsync(int customerId);
        Task<List<Order>> GetByArtistAsync(int artistProfileId);
    }
}
