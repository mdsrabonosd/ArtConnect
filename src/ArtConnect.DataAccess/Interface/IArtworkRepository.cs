using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IArtworkRepository : IRepository<Artwork>
    {
        Task<List<Artwork>> GetAllWithDetailsAsync();
        Task<Artwork?> GetByIdWithDetailsAsync(int id);
    }
}
