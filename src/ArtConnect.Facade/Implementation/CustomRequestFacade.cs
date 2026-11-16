using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class CustomRequestFacade : ICustomRequestFacade
    {
        private readonly ICustomRequestRepository _repository;

        public CustomRequestFacade(ICustomRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomRequest>> GetCustomRequestsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CustomRequest?> GetCustomRequestByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddCustomRequestAsync(CustomRequest item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Custom request created.");
        }

        public async Task<OperationResult> UpdateCustomRequestAsync(CustomRequest item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Custom request updated.")
                : OperationResult.Fail("Custom request not found.");
        }

        public async Task<OperationResult> DeleteCustomRequestAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Custom request deleted.")
                : OperationResult.Fail("Custom request not found.");
        }
    }
}
