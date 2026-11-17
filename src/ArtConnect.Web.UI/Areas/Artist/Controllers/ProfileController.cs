using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    public class ProfileController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;

        public ProfileController(IArtistPanelFacade artistFacade)
        {
            _artistFacade = artistFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var profile = await _artistFacade.GetProfileAsync(CurrentUserId);
            if (profile == null)
                return RedirectToAction("Index", "Dashboard");

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Bio,Skills,ExperienceYears,ProfilePhotoUrl")] ArtistProfile profile)
        {
            if (!ModelState.IsValid)
                return View(profile);

            var result = await _artistFacade.UpdateProfileAsync(CurrentUserId, profile);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(profile);
            }

            SetSuccess(result.Message);
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
