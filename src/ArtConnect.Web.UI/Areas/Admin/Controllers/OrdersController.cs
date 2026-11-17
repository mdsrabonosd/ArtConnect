using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        private readonly IOrderFacade _facade;
        private readonly IUserFacade _userFacade;
        private readonly IArtistProfileFacade _artistFacade;
        private readonly IArtistProposalFacade _proposalFacade;

        public OrdersController(IOrderFacade facade,
            IUserFacade userFacade,
            IArtistProfileFacade artistFacade,
            IArtistProposalFacade proposalFacade)
        {
            _facade = facade;
            _userFacade = userFacade;
            _artistFacade = artistFacade;
            _proposalFacade = proposalFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetOrdersAsync();
            return View(items);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var item = await _facade.GetOrderByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new Order { OrderDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,ArtistProfileId,ProposalId,OrderType,TotalAmount,CommissionAmount,Status,OrderDate")] Order item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.AddOrderAsync(item);
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
            var item = await _facade.GetOrderByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,CustomerId,ArtistProfileId,ProposalId,OrderType,TotalAmount,CommissionAmount,Status,OrderDate")] Order item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.UpdateOrderAsync(item);
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
            var item = await _facade.GetOrderByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _facade.DeleteOrderAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var users = await _userFacade.GetUsersAsync();
            ViewBag.Customers = ToItems(
                users.Where(u => u.Role == AppRoles.Customer),
                u => u.User.Id, u => $"{u.User.FullName} ({u.User.Email})");

            var artists = await _artistFacade.GetArtistProfilesAsync();
            ViewBag.Artists = ToItems(artists, a => a.Id, a => a.User.FullName);

            var proposals = await _proposalFacade.GetArtistProposalsAsync();
            ViewBag.Proposals = ToItems(proposals, p => p.Id,
                p => $"#{p.Id} - {p.CustomRequest.Title} - {p.ArtistProfile.User.FullName}");
        }
    }
}
