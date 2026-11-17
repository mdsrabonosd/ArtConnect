using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;

namespace ArtConnect.Facade.Implementation
{
    public class CommentFacade : ICommentFacade
    {
        private readonly ICommentRepository _repository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentFacade(
            ICommentRepository repository,
            IArtworkRepository artworkRepository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _artworkRepository = artworkRepository;
            _userManager = userManager;
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddCommentAsync(Comment item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var error = await ValidateAsync(item, isNew: true);
            if (error != null)
                return OperationResult.Fail(error);


            await _repository.AddAsync(item);
            return OperationResult.Ok("Comment created.");
        }

        public async Task<OperationResult> UpdateCommentAsync(Comment item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var existing = await _repository.GetByIdAsync(item.Id);
            if (existing == null)
                return OperationResult.Fail("Comment not found.");


            var error = await ValidateAsync(item, isNew: false);
            if (error != null)
                return OperationResult.Fail(error);

            existing.ArtworkId = item.ArtworkId;
            existing.UserId = item.UserId;
            existing.Text = item.Text;

            await _repository.UpdateAsync(existing);
            return OperationResult.Ok("Comment updated.");
        }

        public async Task<OperationResult> DeleteCommentAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Comment not found.");


            await _repository.DeleteAsync(id);
            return OperationResult.Ok("Comment deleted.");
        }

        private async Task<string?> ValidateAsync(Comment item, bool isNew)
        {
            var text = (item.Text ?? string.Empty).Trim();
            if (text.Length < 1 || text.Length > 300)
                return "A comment must be between 1 and 300 characters.";

            item.Text = text;

            if (await _artworkRepository.GetByIdAsync(item.ArtworkId) == null)
                return "Please select a valid artwork.";

            var user = await _userManager.FindByIdAsync(item.UserId.ToString());
            if (user == null || user.IsDeleted)
                return "Please select a valid user.";

            return null;
        }
    }
}
