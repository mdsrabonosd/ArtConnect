using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface ICustomRequestRepository : IRepository<CustomRequest>
    {
        Task<List<CustomRequest>> GetAllWithDetailsAsync();
        Task<CustomRequest?> GetByIdWithDetailsAsync(int id);

        Task<List<CustomRequest>> GetByCustomerAsync(int customerId);
        Task<List<CustomRequest>> GetOpenAsync(int? categoryId);
    }
}
