using ArtConnect.Framework;
using ArtConnect.Web.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppRoles.Customer)]
    public abstract class CustomerBaseController : AppControllerBase
    {
    }
}
