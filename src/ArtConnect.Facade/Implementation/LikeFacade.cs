using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class LikeFacade : ILikeFacade
    {
        private readonly ILikeRepository _repository;

        public LikeFacade(ILikeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Like>> GetLikesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Like?> GetLikeByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddLikeAsync(Like item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Like created.");
        }

        public async Task<OperationResult> UpdateLikeAsync(Like item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Like updated.")
                : OperationResult.Fail("Like not found.");
        }

        public async Task<OperationResult> DeleteLikeAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Like deleted.")
                : OperationResult.Fail("Like not found.");
        }
    }
}
