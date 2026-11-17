using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using ArtConnect.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtConnect.Web.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserFacade _userFacade;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserFacade userFacade)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userFacade = userFacade;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // ---------------- Login ----------------

        [HttpGet]
        [Route(UrlRewrite.Account.Login)]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route(UrlRewrite.Account.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userFacade.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // A blocked user cannot log in.
            if (user.IsBlocked)
            {
                ModelState.AddModelError(string.Empty, "This account is blocked. Please contact the administrator.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in a few minutes.");
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);

            // Role-based landing page
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (await _userManager.IsInRoleAsync(user, AppRoles.Artist))
                return RedirectToAction("Index", "Dashboard", new { area = "Artist" });

            if (await _userManager.IsInRoleAsync(user, AppRoles.Customer))
                return RedirectToAction("Index", "Dashboard", new { area = "Customer" });

            return RedirectToAction("Index", "Home");
        }

        // ---------------- Registration ----------------

        [HttpGet]
        [Route(UrlRewrite.Account.Register)]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel { Role = AppRoles.Customer });
        }

        [HttpPost]
        [Route(UrlRewrite.Account.Register)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userFacade.RegisterAsync(
                user, model.Password, model.Role, model.Bio, model.ExperienceYears);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            // Sign the new user in right away.
            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["Success"] = "Welcome to ArtConnect! Your account is ready.";

            return model.Role == AppRoles.Artist
                ? RedirectToAction("Index", "Dashboard", new { area = "Artist" })
                : RedirectToAction("Index", "Dashboard", new { area = "Customer" });
        }

        // ---------------- Logout ----------------

        [HttpPost]
        [Route(UrlRewrite.Account.Logout)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        // ---------------- My profile ----------------

        [Authorize]
        [HttpGet]
        [Route(UrlRewrite.Account.Profile)]
        public async Task<IActionResult> Profile()
        {
            var item = await _userFacade.GetUserByIdAsync(CurrentUserId);
            if (item == null)
                return RedirectToAction(nameof(Login));

            return View(new ProfileViewModel
            {
                FullName = item.User.FullName,
                Email = item.User.Email ?? string.Empty,
                PhoneNumber = item.User.PhoneNumber ?? string.Empty,
                Role = item.Role
            });
        }

        [Authorize]
        [HttpPost]
        [Route(UrlRewrite.Account.Profile)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            // Email and role are shown only, so always read them again from the database.
            var item = await _userFacade.GetUserByIdAsync(CurrentUserId);
            if (item == null)
                return RedirectToAction(nameof(Login));

            model.Email = item.User.Email ?? string.Empty;
            model.Role = item.Role;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userFacade.UpdateOwnProfileAsync(CurrentUserId, model.FullName, model.PhoneNumber);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Profile));
        }

        // ---------------- Change password ----------------

        [Authorize]
        [HttpGet]
        [Route(UrlRewrite.Account.ChangePassword)]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [Authorize]
        [HttpPost]
        [Route(UrlRewrite.Account.ChangePassword)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userFacade.ChangePasswordAsync(CurrentUserId, model.CurrentPassword, model.NewPassword);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            // Changing the password changes the security stamp, so sign the user in again.
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Profile));
        }

        // ---------------- Access denied ----------------

        [Route(UrlRewrite.Account.AccessDenied)]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
