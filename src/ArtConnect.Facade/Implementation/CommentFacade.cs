using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class CommentFacade : ICommentFacade
    {
        private readonly ICommentRepository _repository;

        public CommentFacade(ICommentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddCommentAsync(Comment item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Comment created.");
        }

        public async Task<OperationResult> UpdateCommentAsync(Comment item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Comment updated.")
                : OperationResult.Fail("Comment not found.");
        }

        public async Task<OperationResult> DeleteCommentAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Comment deleted.")
                : OperationResult.Fail("Comment not found.");
        }
    }
}
