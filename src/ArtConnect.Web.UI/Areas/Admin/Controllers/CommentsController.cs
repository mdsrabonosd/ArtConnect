using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Admin.Controllers
{
    public class CommentsController : AdminBaseController
    {
        private readonly ICommentFacade _facade;
        private readonly IArtworkFacade _artworkFacade;
        private readonly IUserFacade _userFacade;

        public CommentsController(ICommentFacade facade,
            IArtworkFacade artworkFacade,
            IUserFacade userFacade)
        {
            _facade = facade;
            _artworkFacade = artworkFacade;
            _userFacade = userFacade;
        }

        // Get all
        public async Task<IActionResult> Index()
        {
            var items = await _facade.GetCommentsAsync();
            return View(items);
        }

        // Get by id
        public async Task<IActionResult> Details(int id)
        {
            var item = await _facade.GetCommentByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // Insert
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new Comment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtworkId,UserId,Text")] Comment item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.AddCommentAsync(item);
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
            var item = await _facade.GetCommentByIdAsync(id);
            if (item == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,ArtworkId,UserId,Text")] Comment item)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(item);
            }

            var result = await _facade.UpdateCommentAsync(item);
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
            var item = await _facade.GetCommentByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _facade.DeleteCommentAsync(id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var artworks = await _artworkFacade.GetArtworksAsync();
            ViewBag.Artworks = ToItems(artworks, a => a.Id, a => a.Title);

            var users = await _userFacade.GetUsersAsync();
            ViewBag.Users = ToItems(users, u => u.User.Id, u => $"{u.User.FullName} ({u.User.Email})");
        }
    }
}
