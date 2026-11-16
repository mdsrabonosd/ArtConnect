using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}
