using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    public class DashboardController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;

        public DashboardController(IArtistPanelFacade artistFacade)
        {
            _artistFacade = artistFacade;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _artistFacade.GetDashboardAsync(CurrentUserId);
            if (dashboard == null)
                return View("NoProfile");

            return View(dashboard);
        }
    }
}
