using ArtConnect.Entities;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IUserFacade
    {
        Task<List<UserListItem>> GetUsersAsync();
        Task<UserListItem?> GetUserByIdAsync(int id);
        Task<ApplicationUser?> FindByEmailAsync(string email);

        Task<OperationResult> CreateUserAsync(ApplicationUser user, string password, string role);
        Task<OperationResult> UpdateUserAsync(int id, string fullName, string? phoneNumber, string role, int currentAdminId);
        Task<OperationResult> SetBlockedAsync(int id, bool blocked, int currentAdminId);
        Task<OperationResult> DeleteUserAsync(int id, int currentAdminId);
    }
}
