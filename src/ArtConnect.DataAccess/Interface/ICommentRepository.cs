using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetAllWithDetailsAsync();
        Task<Comment?> GetByIdWithDetailsAsync(int id);
    }
}
