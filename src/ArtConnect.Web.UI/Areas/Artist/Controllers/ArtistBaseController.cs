using ArtConnect.Framework;
using ArtConnect.Web.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    [Area("Artist")]
    [Authorize(Roles = AppRoles.Artist)]
    public abstract class ArtistBaseController : AppControllerBase
    {
    }
}
