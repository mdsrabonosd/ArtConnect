using ArtConnect.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        /// <summary>Builds dropdown items. The selected value is set by the asp-for tag helper.</summary>
        protected static List<SelectListItem> ToItems<T>(
            IEnumerable<T> source, Func<T, int> value, Func<T, string> text)
        {
            return source
                .Select(x => new SelectListItem { Value = value(x).ToString(), Text = text(x) })
                .ToList();
        }
    }
}
