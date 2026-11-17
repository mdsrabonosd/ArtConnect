using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    public class ArtworksController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly IImageUploadService _imageUploadService;

        public ArtworksController(
            IArtistPanelFacade artistFacade,
            ICategoryFacade categoryFacade,
            IImageUploadService imageUploadService)
        {
            _artistFacade = artistFacade;
            _categoryFacade = categoryFacade;
            _imageUploadService = imageUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var artworks = await _artistFacade.GetMyArtworksAsync(CurrentUserId);
            return View(artworks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var artwork = await _artistFacade.GetMyArtworkAsync(CurrentUserId, id);
            if (artwork == null)
                return NotFound();

            return View(artwork);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new Artwork());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CategoryId,Title,Description,Medium,Size,Price,IsOriginal,Status")] Artwork artwork,
            IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                ModelState.AddModelError("imageFile", "Please choose an image (JPG or PNG, up to 5 MB).");

            string? imageUrl = null;
            if (ModelState.IsValid && imageFile != null)
            {
                var upload = await _imageUploadService.SaveAsync(imageFile, "artworks");
                if (upload.Error != null)
                    ModelState.AddModelError("imageFile", upload.Error);
                else
                    imageUrl = upload.Url;
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(artwork);
            }

            var result = await _artistFacade.AddArtworkAsync(CurrentUserId, artwork, imageUrl);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadCategoriesAsync();
                return View(artwork);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artwork = await _artistFacade.GetMyArtworkAsync(CurrentUserId, id);
            if (artwork == null)
                return NotFound();

            await LoadCategoriesAsync();
            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,CategoryId,Title,Description,Medium,Size,Price,IsOriginal,Status")] Artwork artwork,
            IFormFile? imageFile)
        {
            string? newImageUrl = null;
            if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
            {
                var upload = await _imageUploadService.SaveAsync(imageFile, "artworks");
                if (upload.Error != null)
                    ModelState.AddModelError("imageFile", upload.Error);
                else
                    newImageUrl = upload.Url;
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(artwork);
            }

            var result = await _artistFacade.UpdateArtworkAsync(CurrentUserId, artwork, newImageUrl);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadCategoriesAsync();
                return View(artwork);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var artwork = await _artistFacade.GetMyArtworkAsync(CurrentUserId, id);
            if (artwork == null)
                return NotFound();

            return View(artwork);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _artistFacade.DeleteArtworkAsync(CurrentUserId, id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryFacade.GetCategoriesAsync();
            ViewBag.Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }
    }
}
