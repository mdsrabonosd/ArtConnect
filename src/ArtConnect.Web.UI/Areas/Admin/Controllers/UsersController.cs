using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using ArtConnect.Web.UI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        private readonly IUserFacade _userFacade;

        public UsersController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var users = await _userFacade.GetUsersAsync();
            ViewBag.CurrentUserId = CurrentUserId;
            return View(users);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userFacade.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // Insert
        [HttpGet]
        public IActionResult Create()
        {
            LoadRoles(AppRoles.Customer);
            return View(new UserCreateViewModel { Role = AppRoles.Customer });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadRoles(model.Role);
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userFacade.CreateUserAsync(user, model.Password, model.Role);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                LoadRoles(model.Role);
                return View(model);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Update
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _userFacade.GetUserByIdAsync(id);
            if (item == null)
                return NotFound();

            LoadRoles(item.Role);
            return View(new UserEditViewModel
            {
                Id = item.User.Id,
                FullName = item.User.FullName,
                Email = item.User.Email ?? string.Empty,
                PhoneNumber = item.User.PhoneNumber,
                Role = item.Role
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadRoles(model.Role);
                return View(model);
            }

            var result = await _userFacade.UpdateUserAsync(
                model.Id, model.FullName, model.PhoneNumber, model.Role, CurrentUserId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                LoadRoles(model.Role);
                return View(model);
            }

            SetSuccess(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Block / unblock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(int id)
        {
            var result = await _userFacade.SetBlockedAsync(id, true, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(int id)
        {
            var result = await _userFacade.SetBlockedAsync(id, false, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        // Soft delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userFacade.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userFacade.DeleteUserAsync(id, CurrentUserId);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private void LoadRoles(string? selected)
        {
            ViewBag.Roles = new SelectList(AppRoles.All, selected);
        }
    }
}
