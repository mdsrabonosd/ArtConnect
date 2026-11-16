using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IArtistProfileRepository : IRepository<ArtistProfile>
    {
        Task<List<ArtistProfile>> GetAllWithDetailsAsync();
        Task<ArtistProfile?> GetByIdWithDetailsAsync(int id);
        Task<ArtistProfile?> GetByUserIdAsync(int userId);
    }
}
