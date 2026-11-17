using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    public class OrdersController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;
        private readonly IImageUploadService _imageUploadService;

        public OrdersController(IArtistPanelFacade artistFacade, IImageUploadService imageUploadService)
        {
            _artistFacade = artistFacade;
            _imageUploadService = imageUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _artistFacade.GetMyOrdersAsync(CurrentUserId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _artistFacade.GetMyOrderAsync(CurrentUserId, id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            var result = await _artistFacade.StartOrderAsync(CurrentUserId, id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile? photo, string? note)
        {
            if (photo == null || photo.Length == 0)
            {
                SetError("Please choose a photo (JPG or PNG, up to 5 MB).");
                return RedirectToAction(nameof(Details), new { id });
            }

            var upload = await _imageUploadService.SaveAsync(photo, "progress");
            if (upload.Error != null || upload.Url == null)
            {
                SetError(upload.Error ?? "The photo could not be saved.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _artistFacade.AddProgressPhotoAsync(CurrentUserId, id, upload.Url, note);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deliver(int id)
        {
            var result = await _artistFacade.DeliverOrderAsync(CurrentUserId, id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
