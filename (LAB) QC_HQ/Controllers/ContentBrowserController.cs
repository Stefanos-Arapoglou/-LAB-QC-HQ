/*NOTES
 * 
This Controller was necessary, to handle the TOTALITY of content, since the ContentController is an abstract
class that cannot be instantiated on its own. This controller serves as a router to the appropriate Controllers,
and also handles listing, activation, and deactivation for the TOTALITY of content, regardless of type.

SUMMARY:
    1) HANDLES BROWSING of ALL content types
    2) REROUTES to appropriate Edit and Details actions based on content type
    3) HANDLES DELETION of content
    4) HANDLES ACTIVATION/DEACTIVATION of content

 */

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

        // Helper to get the current user's ID, used mainly for perimssioning checks
        protected string CurrentUserId => _userManager.GetUserId(User)!;



        // GET: /Content
        // Lists all content, with different views for Admins and regular users
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                //Only Admins can see inactive content! 
                //Also has permissioning check, admins may not view all content in future (?)
                var content = _contentService
                    .GetAllContentIncludingInactive()
                    .Where(c => _authService.CanView(c.ContentId, CurrentUserId))
                    .ToList();

                return View(content);
            }
            else
            {
                //Normal users only see active content they have permission to view
                var content = _contentService
                    .GetBrowsableContent()
                    .Where(c => _authService.CanView(c.ContentId, CurrentUserId))
                    .ToList();

                return View(content);
            }
        }



        // URL: /ContentBrowse/Edit/5
        // Reroutes to appropriate Edit action based on content type
        public async Task<IActionResult> Edit(int id)
        {
            // Check permissions
            if (!_authService.CanView(id, CurrentUserId))
                return Forbid();

            // Get content type
            var contentType = await _contentService.GetContentTypeAsync(id);

            // Route to appropriate controller
            return contentType switch
            {
                ContentType.KnowHow => RedirectToAction("Edit", "KnowHow", new { id }),
                ContentType.Educational => RedirectToAction("Edit", "Educational", new { id }),
                ContentType.Announcement => RedirectToAction("Edit", "Announcement", new { id }),
                ContentType.File => RedirectToAction("Edit", "File", new { id }),
                // "Item" and "Document" don't exist in your enum - remove them!
                _ => NotFound($"Content type not found for ID: {id}")
            };
        }



        // URL: /ContentBrowse/Details/5
        // Reroutes to appropriate Details action based on content type
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



        // POST: /ContentBrowse/Delete/5
        // Deletes content after checking permissions
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



        // POST: /ContentBrowse/Activate/5
        // Activates content (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            await _contentService.ActivateContentAsync(id);
            return RedirectToAction(nameof(Index));
        }



        // POST: /ContentBrowse/Deactivate/5
        // Deactivates content (Admin only)
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
