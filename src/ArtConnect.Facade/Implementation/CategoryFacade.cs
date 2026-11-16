using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class CategoryFacade : ICategoryFacade
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICustomRequestRepository _customRequestRepository;

        public CategoryFacade(
            ICategoryRepository categoryRepository,
            IArtworkRepository artworkRepository,
            ICustomRequestRepository customRequestRepository)
        {
            _categoryRepository = categoryRepository;
            _artworkRepository = artworkRepository;
            _customRequestRepository = customRequestRepository;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddCategoryAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            if (string.IsNullOrWhiteSpace(category.Name))
                return OperationResult.Fail("Category name is required.");

            category.Name = category.Name.Trim();

            if (await _categoryRepository.NameExistsAsync(category.Name))
                return OperationResult.Fail("A category with this name already exists.");

            await _categoryRepository.AddAsync(category);
            return OperationResult.Ok("Category created.");
        }

        public async Task<OperationResult> UpdateCategoryAsync(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            if (string.IsNullOrWhiteSpace(category.Name))
                return OperationResult.Fail("Category name is required.");

            category.Name = category.Name.Trim();

            if (await _categoryRepository.NameExistsAsync(category.Name, category.Id))
                return OperationResult.Fail("A category with this name already exists.");

            var updated = await _categoryRepository.UpdateAsync(category);
            return updated
                ? OperationResult.Ok("Category updated.")
                : OperationResult.Fail("Category not found.");
        }

        public async Task<OperationResult> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return OperationResult.Fail("Category not found.");

            // Business rule: a category that is already used cannot be deleted.
            if (await _artworkRepository.AnyAsync(a => a.CategoryId == id) ||
                await _customRequestRepository.AnyAsync(r => r.CategoryId == id))
            {
                return OperationResult.Fail("This category is used by artworks or custom requests, so it cannot be deleted.");
            }

            await _categoryRepository.DeleteAsync(id);
            return OperationResult.Ok("Category deleted.");
        }
    }
}
