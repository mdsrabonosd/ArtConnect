using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Controllers
{
    public class GalleryController : AppControllerBase
    {
        private readonly IGalleryFacade _galleryFacade;
        private readonly ICategoryFacade _categoryFacade;

        public GalleryController(IGalleryFacade galleryFacade, ICategoryFacade categoryFacade)
        {
            _galleryFacade = galleryFacade;
            _categoryFacade = categoryFacade;
        }

        // Browse, search and filter (10 to 12 per page)
        public async Task<IActionResult> Index(ArtworkFilter filter)
        {
            var model = new GalleryIndexViewModel { Filter = filter };

            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
            {
                model.Error = "The minimum price cannot be more than the maximum price.";
                filter.MinPrice = null;
                filter.MaxPrice = null;
            }

            model.Result = await _galleryFacade.SearchAsync(filter);
            model.Categories = (await _categoryFacade.GetCategoriesAsync()).OrderBy(c => c.Name).ToList();
            model.Mediums = await _galleryFacade.GetMediumsAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _galleryFacade.GetDetailsAsync(id, CurrentUserIdOrNull);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var result = await _galleryFacade.ToggleLikeAsync(id, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWishlist(int id)
        {
            var result = await _galleryFacade.ToggleWishlistAsync(id, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string? text)
        {
            var result = await _galleryFacade.AddCommentAsync(id, CurrentUserId, text ?? string.Empty);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int artworkId)
        {
            var result = await _galleryFacade.DeleteCommentAsync(id, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id = artworkId });
        }
    }
}
