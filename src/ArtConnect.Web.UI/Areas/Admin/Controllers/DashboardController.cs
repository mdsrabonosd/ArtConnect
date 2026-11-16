using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IDashboardFacade _dashboardFacade;

        public DashboardController(IDashboardFacade dashboardFacade)
        {
            _dashboardFacade = dashboardFacade;
        }

        public async Task<IActionResult> Index()
        {
            var summary = await _dashboardFacade.GetSummaryAsync();
            return View(summary);
        }
    }
}
