using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class ReviewsController : AdminBaseController
    {
        private readonly IReviewFacade _facade;
        private readonly IOrderFacade _orderFacade;

        public ReviewsController(IReviewFacade facade,
            IOrderFacade orderFacade)
        {
            _facade = facade;
            _orderFacade = orderFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetReviewsAsync();
            return View(items);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var item = await _facade.GetReviewByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new Review { Rating = 5 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,Rating,Comment")] Review item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.AddReviewAsync(item);
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
            var item = await _facade.GetReviewByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,OrderId,Rating,Comment")] Review item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.UpdateReviewAsync(item);
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
            var item = await _facade.GetReviewByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _facade.DeleteReviewAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var orders = await _orderFacade.GetOrdersAsync();
            ViewBag.Orders = ToItems(
                orders.Where(o => o.Status == OrderStatus.Delivered),
                o => o.Id, o => $"#{o.Id} - {o.Customer.FullName} to {o.ArtistProfile.User.FullName}");
        }
    }
}
