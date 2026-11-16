using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface ICommentFacade
    {
        Task<List<Comment>> GetCommentsAsync();
        Task<Comment?> GetCommentByIdAsync(int id);
        Task<OperationResult> AddCommentAsync(Comment item);
        Task<OperationResult> UpdateCommentAsync(Comment item);
        Task<OperationResult> DeleteCommentAsync(int id);
    }
}
