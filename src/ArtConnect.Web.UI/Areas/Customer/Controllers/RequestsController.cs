using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Models;
using ArtConnect.Web.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    public class RequestsController : CustomerBaseController
    {
        private readonly ICustomerFacade _customerFacade;
        private readonly ICategoryFacade _categoryFacade;
        private readonly IImageUploadService _imageUploadService;

        public RequestsController(
            ICustomerFacade customerFacade,
            ICategoryFacade categoryFacade,
            IImageUploadService imageUploadService)
        {
            _customerFacade = customerFacade;
            _categoryFacade = categoryFacade;
            _imageUploadService = imageUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _customerFacade.GetMyRequestsAsync(CurrentUserId);
            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _customerFacade.GetMyRequestAsync(id, CurrentUserId);
            if (request == null)
                return NotFound();

            var proposals = await _customerFacade.GetProposalsForRequestAsync(id, CurrentUserId);
            return View(new RequestDetailsViewModel { Request = request, Proposals = proposals });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new CustomRequest { Deadline = DateTime.Today.AddDays(15) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CategoryId,Title,Description,CanvasSize,BudgetMin,BudgetMax,Deadline")] CustomRequest request,
            IFormFile? referenceImage)
        {
            if (ModelState.IsValid && referenceImage != null && referenceImage.Length > 0)
            {
                var upload = await _imageUploadService.SaveAsync(referenceImage, "requests");
                if (upload.Error != null)
                    ModelState.AddModelError("referenceImage", upload.Error);
                else
                    request.ReferenceImageUrl = upload.Url;
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(request);
            }

            var result = await _customerFacade.CreateRequestAsync(request, CurrentUserId);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadCategoriesAsync();
                return View(request);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _customerFacade.CancelRequestAsync(id, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // id is the proposal id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var result = await _customerFacade.AcceptProposalAsync(id, CurrentUserId);
            if (result.Success)
            {
                SetSuccess(result.Message);
                return RedirectToAction("Index", "Orders");
            }

            SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryFacade.GetCategoriesAsync();
            ViewBag.Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }
    }
}
