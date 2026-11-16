using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface ICustomRequestFacade
    {
        Task<List<CustomRequest>> GetCustomRequestsAsync();
        Task<CustomRequest?> GetCustomRequestByIdAsync(int id);
        Task<OperationResult> AddCustomRequestAsync(CustomRequest item);
        Task<OperationResult> UpdateCustomRequestAsync(CustomRequest item);
        Task<OperationResult> DeleteCustomRequestAsync(int id);
    }
}
