using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    public class CartController : CustomerBaseController
    {
        private readonly ICustomerFacade _customerFacade;

        public CartController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
        }

        public async Task<IActionResult> Index()
        {
            var ids = SessionCart.Get(HttpContext.Session);
            var artworks = await _customerFacade.GetCartArtworksAsync(ids);
            return View(artworks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var artwork = (await _customerFacade.GetCartArtworksAsync(new[] { id })).FirstOrDefault();

            if (artwork == null)
                SetError("Artwork not found.");
            else if (artwork.Status != ArtworkStatus.Available)
                SetError("This painting has already been sold.");
            else if (artwork.ArtistProfile.UserId == CurrentUserId)
                SetError("You cannot buy your own artwork.");
            else if (SessionCart.Add(HttpContext.Session, id))
                SetSuccess("Added to your cart.");
            else
                SetError("This painting is already in your cart.");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            SessionCart.Remove(HttpContext.Session, id);
            SetSuccess("Removed from your cart.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var ids = SessionCart.Get(HttpContext.Session);
            var result = await _customerFacade.CheckoutAsync(CurrentUserId, ids);

            if (!result.Success)
            {
                SetError(result.Message);
                return RedirectToAction(nameof(Index));
            }

            SessionCart.Clear(HttpContext.Session);
            SetSuccess(result.Message);
            return RedirectToAction("Index", "Orders");
        }
    }
}
