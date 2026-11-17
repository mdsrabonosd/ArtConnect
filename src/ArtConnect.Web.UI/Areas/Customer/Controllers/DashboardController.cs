using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Customer.Controllers
{
    public class DashboardController : CustomerBaseController
    {
        private readonly ICustomerFacade _customerFacade;

        public DashboardController(ICustomerFacade customerFacade)
        {
            _customerFacade = customerFacade;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _customerFacade.GetDashboardAsync(CurrentUserId);
            return View(dashboard);
        }
    }
}
