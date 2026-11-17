using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtConnect.Web.UI.Controllers
{
    /// <summary>Small helpers shared by the public, customer and artist controllers.</summary>
    public abstract class AppControllerBase : Controller
    {
        protected int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        protected int? CurrentUserIdOrNull =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        protected void SetSuccess(string message) => TempData["Success"] = message;

        protected void SetError(string message) => TempData["Error"] = message;
    }
}
