using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class CustomRequestsController : AdminBaseController
    {
        private readonly ICustomRequestFacade _facade;
        private readonly IUserFacade _userFacade;
        private readonly ICategoryFacade _categoryFacade;

        public CustomRequestsController(ICustomRequestFacade facade,
            IUserFacade userFacade,
            ICategoryFacade categoryFacade)
        {
            _facade = facade;
            _userFacade = userFacade;
            _categoryFacade = categoryFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetCustomRequestsAsync();
            return View(items);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var item = await _facade.GetCustomRequestByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new CustomRequest { Deadline = DateTime.Today.AddDays(7) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CategoryId,Title,CanvasSize,BudgetMin,BudgetMax,Deadline,Status,ReferenceImageUrl,Description")] CustomRequest item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.AddCustomRequestAsync(item);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync();
                return View(item);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _facade.GetCustomRequestByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,CustomerId,CategoryId,Title,CanvasSize,BudgetMin,BudgetMax,Deadline,Status,ReferenceImageUrl,Description")] CustomRequest item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.UpdateCustomRequestAsync(item);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await LoadLookupsAsync();
                return View(item);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _facade.GetCustomRequestByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _facade.DeleteCustomRequestAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var users = await _userFacade.GetUsersAsync();
            ViewBag.Customers = ToItems(
                users.Where(u => u.Role == AppRoles.Customer),
                u => u.User.Id, u => $"{u.User.FullName} ({u.User.Email})");

            var categories = await _categoryFacade.GetCategoriesAsync();
            ViewBag.Categories = ToItems(categories.OrderBy(c => c.Name), c => c.Id, c => c.Name);
        }
    }
}
