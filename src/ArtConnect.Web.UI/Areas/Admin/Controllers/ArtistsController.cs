using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class ArtistsController : AdminBaseController
    {
        private readonly IArtistProfileFacade _artistFacade;

        public ArtistsController(IArtistProfileFacade artistFacade)
        {
            _artistFacade = artistFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var artists = await _artistFacade.GetArtistProfilesAsync();
            return View(artists);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var artist = await _artistFacade.GetArtistProfileByIdAsync(id);
            if (artist == null)
                return NotFound();

            return View(artist);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadUsersAsync();
            return View(new ArtistProfile());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserId,Bio,Skills,ExperienceYears,ProfilePhotoUrl")] ArtistProfile profile)
        {
            if (!ModelState.IsValid)
            {
                await LoadUsersAsync();
                return View(profile);
            }

            var result = await _artistFacade.AddArtistProfileAsync(profile);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadUsersAsync();
                return View(profile);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _artistFacade.GetArtistProfileByIdAsync(id);
            if (artist == null)
                return NotFound();

            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Bio,Skills,ExperienceYears,ProfilePhotoUrl")] ArtistProfile profile)
        {
            if (!ModelState.IsValid)
            {
                var current = await _artistFacade.GetArtistProfileByIdAsync(profile.Id);
                if (current == null)
                    return NotFound();

                // Show the account name again; keep the values the admin typed.
                profile.User = current.User;
                profile.UserId = current.UserId;
                return View(profile);
            }

            var result = await _artistFacade.UpdateArtistProfileAsync(profile);
            if (!result.Success)
            {
                SetError(result.Message);
                return RedirectToAction(nameof(Edit), new { id = profile.Id });
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Verify / remove verification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            var result = await _artistFacade.SetVerifiedAsync(id, true, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unverify(int id)
        {
            var result = await _artistFacade.SetVerifiedAsync(id, false, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _artistFacade.GetArtistProfileByIdAsync(id);
            if (artist == null)
                return NotFound();

            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _artistFacade.DeleteArtistProfileAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadUsersAsync()
        {
            var users = await _artistFacade.GetAvailableArtistUsersAsync();
            ViewBag.Users = users
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FullName} ({u.Email})"
                })
                .ToList();
        }
    }
}
