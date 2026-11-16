using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class ArtworksController : AdminBaseController
    {
        private readonly IArtworkFacade _artworkFacade;
        private readonly IArtistProfileFacade _artistFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly IImageUploadService _imageUploadService;

        public ArtworksController(
            IArtworkFacade artworkFacade,
            IArtistProfileFacade artistFacade,
            ICategoryFacade categoryFacade,
            IImageUploadService imageUploadService)
        {
            _artworkFacade = artworkFacade;
            _artistFacade = artistFacade;
            _categoryFacade = categoryFacade;
            _imageUploadService = imageUploadService;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var artworks = await _artworkFacade.GetArtworksAsync();
            return View(artworks);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var artwork = await _artworkFacade.GetArtworkByIdAsync(id);
            if (artwork == null)
                return NotFound();

            return View(artwork);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new Artwork());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ArtistProfileId,CategoryId,Title,Description,Medium,Size,Price,IsOriginal,Status")] Artwork artwork,
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
                await LoadLookupsAsync(artwork.ArtistProfileId, artwork.CategoryId);
                return View(artwork);
            }

            var result = await _artworkFacade.AddArtworkAsync(artwork, imageUrl);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync(artwork.ArtistProfileId, artwork.CategoryId);
                return View(artwork);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artwork = await _artworkFacade.GetArtworkByIdAsync(id);
            if (artwork == null)
                return NotFound();

            await LoadLookupsAsync(artwork.ArtistProfileId, artwork.CategoryId);
            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,ArtistProfileId,CategoryId,Title,Description,Medium,Size,Price,IsOriginal,Status")] Artwork artwork,
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
                await LoadLookupsAsync(artwork.ArtistProfileId, artwork.CategoryId);
                return View(artwork);
            }

            var result = await _artworkFacade.UpdateArtworkAsync(artwork, newImageUrl);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync(artwork.ArtistProfileId, artwork.CategoryId);
                return View(artwork);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var artwork = await _artworkFacade.GetArtworkByIdAsync(id);
            if (artwork == null)
                return NotFound();

            return View(artwork);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _artworkFacade.DeleteArtworkAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync(int? artistId = null, int? categoryId = null)
        {
            var artists = await _artistFacade.GetArtistProfilesAsync();
            ViewBag.Artists = artists
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.User.FullName,
                    Selected = a.Id == artistId
                })
                .ToList();

            var categories = await _categoryFacade.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories.OrderBy(c => c.Name), "Id", "Name", categoryId);
        }
    }
}
