using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    public class WishlistController : CustomerBaseController
    {
        private readonly ICustomerFacade _customerFacade;

        public WishlistController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _customerFacade.GetWishlistAsync(CurrentUserId);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int artworkId)
        {
            var result = await _customerFacade.RemoveFromWishlistAsync(artworkId, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
