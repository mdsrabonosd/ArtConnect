using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;

namespace ArtConnect.Facade.Implementation
{
    public class ArtistProfileFacade : IArtistProfileFacade
    {
        private readonly IArtistProfileRepository _profileRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArtistProfileFacade(
            IArtistProfileRepository profileRepository,
            IArtworkRepository artworkRepository,
            UserManager<ApplicationUser> userManager)
        {
            _profileRepository = profileRepository;
            _artworkRepository = artworkRepository;
            _userManager = userManager;
        }

        public async Task<List<ArtistProfile>> GetArtistProfilesAsync()
        {
            return await _profileRepository.GetAllWithDetailsAsync();
        }

        public async Task<ArtistProfile?> GetArtistProfileByIdAsync(int id)
        {
            return await _profileRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<List<ApplicationUser>> GetAvailableArtistUsersAsync()
        {
            var artistUsers = await _userManager.GetUsersInRoleAsync(AppRoles.Artist);
            var profiles = await _profileRepository.GetAllAsync();
            var usedUserIds = profiles.Select(p => p.UserId).ToHashSet();

            return artistUsers
                .Where(u => !u.IsDeleted && !u.IsBlocked && !usedUserIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToList();
        }

        public async Task<OperationResult> AddArtistProfileAsync(ArtistProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (string.IsNullOrWhiteSpace(profile.Bio))
                return OperationResult.Fail("Bio is required.");

            var user = await _userManager.FindByIdAsync(profile.UserId.ToString());
            if (user == null || user.IsDeleted)
                return OperationResult.Fail("Please select a valid user.");

            if (!await _userManager.IsInRoleAsync(user, AppRoles.Artist))
                return OperationResult.Fail("The selected user does not have the Artist role.");

            if (await _profileRepository.GetByUserIdAsync(profile.UserId) != null)
                return OperationResult.Fail("This user already has an artist profile.");

            // A new profile is never verified. Only the admin can verify it.
            profile.IsVerified = false;
            profile.VerifiedAt = null;
            profile.VerifiedByUserId = null;

            await _profileRepository.AddAsync(profile);
            return OperationResult.Ok("Artist profile created.");
        }

        public async Task<OperationResult> UpdateArtistProfileAsync(ArtistProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (string.IsNullOrWhiteSpace(profile.Bio))
                return OperationResult.Fail("Bio is required.");

            var existing = await _profileRepository.GetByIdAsync(profile.Id);
            if (existing == null)
                return OperationResult.Fail("Artist profile not found.");

            // Only these fields can be edited here. UserId and verification stay unchanged.
            existing.Bio = profile.Bio.Trim();
            existing.Skills = profile.Skills;
            existing.ExperienceYears = profile.ExperienceYears;
            existing.ProfilePhotoUrl = profile.ProfilePhotoUrl;

            await _profileRepository.UpdateAsync(existing);
            return OperationResult.Ok("Artist profile updated.");
        }

        public async Task<OperationResult> DeleteArtistProfileAsync(int id)
        {
            var existing = await _profileRepository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Artist profile not found.");

            if (await _artworkRepository.AnyAsync(a => a.ArtistProfileId == id))
                return OperationResult.Fail("This artist still has artworks. Delete the artworks first.");

            await _profileRepository.DeleteAsync(id);
            return OperationResult.Ok("Artist profile deleted.");
        }

        public async Task<OperationResult> SetVerifiedAsync(int id, bool verified, int adminUserId)
        {
            var existing = await _profileRepository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Artist profile not found.");

            existing.IsVerified = verified;
            existing.VerifiedAt = verified ? DateTime.UtcNow : null;
            existing.VerifiedByUserId = verified ? adminUserId : null;

            await _profileRepository.UpdateAsync(existing);
            return OperationResult.Ok(verified ? "Artist verified." : "Artist verification removed.");
        }
    }
}
