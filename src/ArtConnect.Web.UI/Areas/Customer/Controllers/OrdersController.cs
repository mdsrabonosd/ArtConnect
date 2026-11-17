using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    public class OrdersController : CustomerBaseController
    {
        private readonly ICustomerFacade _customerFacade;

        public OrdersController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _customerFacade.GetMyOrdersAsync(CurrentUserId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _customerFacade.GetMyOrderAsync(id, CurrentUserId);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, int rating, string? comment)
        {
            var result = await _customerFacade.AddReviewAsync(id, rating, comment, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
