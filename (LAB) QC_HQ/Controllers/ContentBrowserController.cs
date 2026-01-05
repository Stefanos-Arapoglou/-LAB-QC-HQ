using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _LAB__QC_HQ.Models.Enums;

namespace _LAB__QC_HQ.Controllers
{
    public class ContentBrowseController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IContentAuthorizationService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContentBrowseController(
            IContentService contentService,
            IContentAuthorizationService authService, UserManager<ApplicationUser> userManager)
        {
            _contentService = contentService;
            _authService = authService;
            _userManager = userManager;
        }

        protected string CurrentUserId =>
_userManager.GetUserId(User)!;

        // GET: /Content
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var content = _contentService
                    .GetAllContentIncludingInactive()
                    .Where(c => _authService.CanView(c.ContentId, CurrentUserId))
                    .ToList();

                return View(content);
            }
            else
            {
                var content = _contentService
    .GetBrowsableContent()
    .Where(c => _authService.CanView(c.ContentId, CurrentUserId))
    .ToList();

                return View(content);
            }
        }

        // URL: /ContentBrowse/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Check permissions
            if (!_authService.CanView(id, CurrentUserId))
                return Forbid();

            // Get content type
            var contentType = await _contentService.GetContentTypeAsync(id);

            // Route to appropriate controller
            return contentType switch
            {
                ContentType.KnowHow => RedirectToAction("Details", "KnowHow", new { id }),
                ContentType.Educational => RedirectToAction("Details", "Educational", new { id }),
                ContentType.Announcement => RedirectToAction("Details", "Announcement", new { id }),
                ContentType.File => RedirectToAction("Details", "File", new { id }),
                // "Item" and "Document" don't exist in your enum - remove them!
                _ => NotFound($"Content type not found for ID: {id}")
            };
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.CanDelete(id, CurrentUserId))
                return Forbid();

            try
            {
                // Delete using ContentService
                await _contentService.DeleteContentAsync(id);

                TempData["SuccessMessage"] = "Content deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            await _contentService.ActivateContentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            await _contentService.DeactivateContentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
