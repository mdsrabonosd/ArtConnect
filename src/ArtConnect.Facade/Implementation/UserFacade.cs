using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.Facade.Implementation
{
    public class UserFacade : IUserFacade
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserFacade(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserListItem>> GetUsersAsync()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var result = new List<UserListItem>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserListItem { User = user, Role = roles.FirstOrDefault() ?? "-" });
            }

            return result;
        }

        public async Task<UserListItem?> GetUserByIdAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserListItem { User = user, Role = roles.FirstOrDefault() ?? "-" };
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return (user == null || user.IsDeleted) ? null : user;
        }

        public async Task<OperationResult> CreateUserAsync(ApplicationUser user, string password, string role)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!AppRoles.All.Contains(role))
                return OperationResult.Fail("Please select a valid role.");

            user.UserName = user.Email;
            user.EmailConfirmed = true;
            user.FullName = user.FullName.Trim();

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return OperationResult.Fail(string.Join(" ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);
            return OperationResult.Ok("User created.");
        }

        public async Task<OperationResult> UpdateUserAsync(int id, string fullName, string? phoneNumber, string role, int currentAdminId)
        {
            if (!AppRoles.All.Contains(role))
                return OperationResult.Fail("Please select a valid role.");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted)
                return OperationResult.Fail("User not found.");

            if (id == currentAdminId && role != AppRoles.Admin)
                return OperationResult.Fail("You cannot remove the Admin role from your own account.");

            user.FullName = fullName.Trim();
            user.PhoneNumber = phoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return OperationResult.Fail(string.Join(" ", updateResult.Errors.Select(e => e.Description)));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(role))
            {
                if (currentRoles.Count > 0)
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                await _userManager.AddToRoleAsync(user, role);
            }

            return OperationResult.Ok("User updated.");
        }

        public async Task<OperationResult> SetBlockedAsync(int id, bool blocked, int currentAdminId)
        {
            if (id == currentAdminId)
                return OperationResult.Fail("You cannot block your own account.");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted)
                return OperationResult.Fail("User not found.");

            user.IsBlocked = blocked;
            await _userManager.UpdateAsync(user);

            // A new security stamp signs the user out of any open session.
            if (blocked)
                await _userManager.UpdateSecurityStampAsync(user);

            return OperationResult.Ok(blocked ? "User blocked." : "User unblocked.");
        }

        public async Task<OperationResult> DeleteUserAsync(int id, int currentAdminId)
        {
            if (id == currentAdminId)
                return OperationResult.Fail("You cannot delete your own account.");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted)
                return OperationResult.Fail("User not found.");

            // Soft delete: the row stays in the database but the user can no longer sign in.
            user.IsDeleted = true;
            user.IsBlocked = true;
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);

            return OperationResult.Ok("User deleted.");
        }
    }
}
