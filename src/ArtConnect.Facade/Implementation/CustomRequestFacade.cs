using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;

namespace ArtConnect.Facade.Implementation
{
    public class CustomRequestFacade : ICustomRequestFacade
    {
        private readonly ICustomRequestRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArtistProposalRepository _proposalRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomRequestFacade(
            ICustomRequestRepository repository,
            ICategoryRepository categoryRepository,
            IArtistProposalRepository proposalRepository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _proposalRepository = proposalRepository;
            _userManager = userManager;
        }

        public async Task<List<CustomRequest>> GetCustomRequestsAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<CustomRequest?> GetCustomRequestByIdAsync(int id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddCustomRequestAsync(CustomRequest item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var error = await ValidateAsync(item, isNew: true);
            if (error != null)
                return OperationResult.Fail(error);

            item.Title = item.Title.Trim();

            await _repository.AddAsync(item);
            return OperationResult.Ok("Custom request created.");
        }

        public async Task<OperationResult> UpdateCustomRequestAsync(CustomRequest item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var existing = await _repository.GetByIdAsync(item.Id);
            if (existing == null)
                return OperationResult.Fail("Custom request not found.");


            var error = await ValidateAsync(item, isNew: false);
            if (error != null)
                return OperationResult.Fail(error);

            existing.CustomerId = item.CustomerId;
            existing.CategoryId = item.CategoryId;
            existing.Title = item.Title.Trim();
            existing.Description = item.Description;
            existing.ReferenceImageUrl = item.ReferenceImageUrl;
            existing.CanvasSize = item.CanvasSize;
            existing.BudgetMin = item.BudgetMin;
            existing.BudgetMax = item.BudgetMax;
            existing.Deadline = item.Deadline;
            existing.Status = item.Status;

            await _repository.UpdateAsync(existing);
            return OperationResult.Ok("Custom request updated.");
        }

        public async Task<OperationResult> DeleteCustomRequestAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Custom request not found.");

            if (await _proposalRepository.AnyAsync(p => p.RequestId == id))
                return OperationResult.Fail("This request has proposals, so it cannot be deleted. Set its status to Cancelled instead.");

            await _repository.DeleteAsync(id);
            return OperationResult.Ok("Custom request deleted.");
        }

        private async Task<string?> ValidateAsync(CustomRequest item, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                return "Title is required.";

            if (string.IsNullOrWhiteSpace(item.Description))
                return "Description is required.";

            if (string.IsNullOrWhiteSpace(item.CanvasSize))
                return "Canvas size is required.";

            if (item.BudgetMin <= 0 || item.BudgetMax <= 0)
                return "Budget must be greater than zero.";

            if (item.BudgetMin > item.BudgetMax)
                return "The minimum budget cannot be more than the maximum budget.";

            if (isNew && item.Deadline.Date < DateTime.UtcNow.Date.AddDays(3))
                return "The deadline must be at least 3 days from today.";

            var customer = await _userManager.FindByIdAsync(item.CustomerId.ToString());
            if (customer == null || customer.IsDeleted)
                return "Please select a valid customer.";

            if (await _categoryRepository.GetByIdAsync(item.CategoryId) == null)
                return "Please select a valid category.";

            return null;
        }
    }
}
