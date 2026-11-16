using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IArtistProfileFacade
    {
        Task<List<ArtistProfile>> GetArtistProfilesAsync();
        Task<ArtistProfile?> GetArtistProfileByIdAsync(int id);

        /// <summary>Users with the Artist role who do not have a profile yet.</summary>
        Task<List<ApplicationUser>> GetAvailableArtistUsersAsync();

        Task<OperationResult> AddArtistProfileAsync(ArtistProfile profile);
        Task<OperationResult> UpdateArtistProfileAsync(ArtistProfile profile);
        Task<OperationResult> DeleteArtistProfileAsync(int id);
        Task<OperationResult> SetVerifiedAsync(int id, bool verified, int adminUserId);
    }
}
