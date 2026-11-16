using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IProgressPhotoFacade
    {
        Task<List<ProgressPhoto>> GetProgressPhotosAsync();
        Task<ProgressPhoto?> GetProgressPhotoByIdAsync(int id);
        Task<OperationResult> AddProgressPhotoAsync(ProgressPhoto item);
        Task<OperationResult> UpdateProgressPhotoAsync(ProgressPhoto item);
        Task<OperationResult> DeleteProgressPhotoAsync(int id);
    }
}
