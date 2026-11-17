using ArtConnect.Facade.Interface;
using ArtConnect.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArtConnect.Web.UI.Controllers
{
    public class HomeController : AppControllerBase
    {
        private readonly IGalleryFacade _galleryFacade;
        private readonly ICategoryFacade _categoryFacade;

        public HomeController(IGalleryFacade galleryFacade, ICategoryFacade categoryFacade)
        {
            _galleryFacade = galleryFacade;
            _categoryFacade = categoryFacade;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                Latest = await _galleryFacade.GetLatestAsync(8),
                Categories = (await _categoryFacade.GetCategoriesAsync()).OrderBy(c => c.Name).ToList()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
