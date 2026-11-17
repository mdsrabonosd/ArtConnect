using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class ArtistProposalsController : AdminBaseController
    {
        private readonly IArtistProposalFacade _facade;
        private readonly ICustomRequestFacade _requestFacade;
        private readonly IArtistProfileFacade _artistFacade;

        public ArtistProposalsController(IArtistProposalFacade facade,
            ICustomRequestFacade requestFacade,
            IArtistProfileFacade artistFacade)
        {
            _facade = facade;
            _requestFacade = requestFacade;
            _artistFacade = artistFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetArtistProposalsAsync();
            return View(items);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var item = await _facade.GetArtistProposalByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new ArtistProposal { DeliveryDays = 7, RevisionsIncluded = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,ArtistProfileId,Price,DeliveryDays,RevisionsIncluded,Status,Message")] ArtistProposal item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.AddArtistProposalAsync(item);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync();
                return View(item);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _facade.GetArtistProposalByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,RequestId,ArtistProfileId,Price,DeliveryDays,RevisionsIncluded,Status,Message")] ArtistProposal item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.UpdateArtistProposalAsync(item);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync();
                return View(item);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _facade.GetArtistProposalByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _facade.DeleteArtistProposalAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var requests = await _requestFacade.GetCustomRequestsAsync();
            ViewBag.Requests = ToItems(requests, r => r.Id, r => $"{r.Title} ({r.Customer.FullName})");

            var artists = await _artistFacade.GetArtistProfilesAsync();
            ViewBag.Artists = ToItems(artists, a => a.Id, a => a.User.FullName);
        }
    }
}
