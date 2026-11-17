using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    /// <summary>Open custom requests that an artist can answer with a proposal.</summary>
    public class RequestsController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;
        private readonly ICategoryFacade _categoryFacade;

        public RequestsController(IArtistPanelFacade artistFacade, ICategoryFacade categoryFacade)
        {
            _artistFacade = artistFacade;
            _categoryFacade = categoryFacade;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _categoryFacade.GetCategoriesAsync();
            ViewBag.Categories = categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name, Selected = c.Id == categoryId })
                .ToList();

            var requests = await _artistFacade.GetOpenRequestsAsync(categoryId);
            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await BuildModelAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            [Bind("RequestId,Price,DeliveryDays,RevisionsIncluded,Message")] ArtistProposal form)
        {
            var model = await BuildModelAsync(form.RequestId);
            if (model == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Form = form;
                return View("Details", model);
            }

            var result = await _artistFacade.SendProposalAsync(CurrentUserId, form);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                model.Form = form;
                return View("Details", model);
            }

            SetSuccess(result.Message);
            return RedirectToAction("Index", "Proposals");
        }

        private async Task<OpenRequestViewModel?> BuildModelAsync(int requestId)
        {
            var request = await _artistFacade.GetOpenRequestAsync(requestId);
            if (request == null)
                return null;

            return new OpenRequestViewModel
            {
                Request = request,
                MyProposal = await _artistFacade.GetMyProposalForRequestAsync(CurrentUserId, requestId),
                Form = new ArtistProposal { RequestId = requestId, DeliveryDays = 7, RevisionsIncluded = 1 }
            };
        }
    }
}
