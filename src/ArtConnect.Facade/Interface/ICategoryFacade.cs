using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface ICategoryFacade
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<OperationResult> AddCategoryAsync(Category category);
        Task<OperationResult> UpdateCategoryAsync(Category category);
        Task<OperationResult> DeleteCategoryAsync(int id);
    }
}
