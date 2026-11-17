using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Controllers
{
    public class ArtistsController : AppControllerBase
    {
        private readonly IGalleryFacade _galleryFacade;

        public ArtistsController(IGalleryFacade galleryFacade)
        {
            _galleryFacade = galleryFacade;
        }

        public async Task<IActionResult> Index()
        {
            var artists = await _galleryFacade.GetArtistsAsync();
            return View(artists);
        }

        public async Task<IActionResult> Details(int id)
        {
            var portfolio = await _galleryFacade.GetArtistPortfolioAsync(id);
            if (portfolio == null)
                return NotFound();

            return View(portfolio);
        }
    }
}
