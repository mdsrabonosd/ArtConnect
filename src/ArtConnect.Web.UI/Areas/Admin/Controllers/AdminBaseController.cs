using ArtConnect.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    /// <summary>
    /// Every admin controller inherits this class, so all admin pages
    /// are limited to users in the Admin role.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public abstract class AdminBaseController : Controller
    {
        protected int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        protected void SetSuccess(string message) => TempData["Success"] = message;

        protected void SetError(string message) => TempData["Error"] = message;
    }
}
