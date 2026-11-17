using ArtConnect.Entities;

namespace ArtConnect.DataAccess.Interface
{
    public interface IArtworkRepository : IRepository<Artwork>
    {
        Task<List<Artwork>> GetAllWithDetailsAsync();
        Task<Artwork?> GetByIdWithDetailsAsync(int id);

        /// <summary>Public search. Artworks of blocked or deleted artists are hidden.</summary>
        Task<(List<Artwork> Items, int TotalCount)> SearchAsync(
            string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, string? medium, int page, int pageSize);

        Task<List<Artwork>> GetByArtistAsync(int artistProfileId);
        Task<List<Artwork>> GetLatestAsync(int count);
        Task<List<string>> GetMediumsAsync();
    }
}
