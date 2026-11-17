using ArtConnect.Facade.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ArtConnect.Web.UI.Areas.Artist.Controllers
{
    public class ProposalsController : ArtistBaseController
    {
        private readonly IArtistPanelFacade _artistFacade;

        public ProposalsController(IArtistPanelFacade artistFacade)
        {
            _artistFacade = artistFacade;
        }

        public async Task<IActionResult> Index()
        {
            var proposals = await _artistFacade.GetMyProposalsAsync(CurrentUserId);
            return View(proposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var result = await _artistFacade.WithdrawProposalAsync(CurrentUserId, id);
            if (result.Success) SetSuccess(result.Message); else SetError(result.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
