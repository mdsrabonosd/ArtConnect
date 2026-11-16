using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class ProgressPhotoFacade : IProgressPhotoFacade
    {
        private readonly IProgressPhotoRepository _repository;

        public ProgressPhotoFacade(IProgressPhotoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProgressPhoto>> GetProgressPhotosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProgressPhoto?> GetProgressPhotoByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddProgressPhotoAsync(ProgressPhoto item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Progress photo created.");
        }

        public async Task<OperationResult> UpdateProgressPhotoAsync(ProgressPhoto item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Progress photo updated.")
                : OperationResult.Fail("Progress photo not found.");
        }

        public async Task<OperationResult> DeleteProgressPhotoAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Progress photo deleted.")
                : OperationResult.Fail("Progress photo not found.");
        }
    }
}
