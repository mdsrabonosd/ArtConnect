using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface ILikeFacade
    {
        Task<List<Like>> GetLikesAsync();
        Task<Like?> GetLikeByIdAsync(int id);
        Task<OperationResult> AddLikeAsync(Like item);
        Task<OperationResult> UpdateLikeAsync(Like item);
        Task<OperationResult> DeleteLikeAsync(int id);
    }
}
