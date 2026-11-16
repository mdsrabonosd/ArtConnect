using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly ICategoryFacade _categoryFacade;

        public CategoriesController(ICategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryFacade.GetCategoriesAsync();
            return View(categories);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryFacade.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // Insert
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var result = await _categoryFacade.AddCategoryAsync(category);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(category);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryFacade.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name")] Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var result = await _categoryFacade.UpdateCategoryAsync(category);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(category);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryFacade.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoryFacade.DeleteCategoryAsync(id);

            if (result.Success)
                SetSuccess(result.Message);
            else
                SetError(result.Message);

            return RedirectToAction(nameof(Index));
        }
    }
}
