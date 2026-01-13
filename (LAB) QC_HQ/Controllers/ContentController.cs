/* NOTES
 
This Controller serves as a base class for other content-related controllers, providing shared functionality
since it is abstract and cannot be instantiated on its own, and can have no POST actions.

SUMMARY:
    1) PROVIDES SHARED DEPENDENCY INJECTION for content services, authorization, user management, environment, and item services
    2) OFFERS HELPER METHODS to check user permissions (view, edit, delete) on content
    3) OFFERS HELPER METHOD to get content by ID (NOT USED AS OF NOW)
 
 */

using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{
    public abstract class ContentController : Controller
    {
        protected readonly IContentService _contentService;
        protected readonly IContentAuthorizationService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly IWebHostEnvironment _env;
        protected readonly IItemService _itemService;

        protected ContentController(IContentService contentService, 
            IContentAuthorizationService authService, 
            UserManager<ApplicationUser> userManager, 
            IWebHostEnvironment env,
            IItemService itemService)
        {
            _contentService = contentService;
            _authService = authService;
            _userManager = userManager;
            _env = env;
            _itemService = itemService;
        }

        // Helper to get the current user's ID, used mainly for permissioning checks
        protected string CurrentUserId => _userManager.GetUserId(User)!;



        // Shared method to check if user can view content
        protected bool CanView(int contentId)
        {
            return _authService.CanView(contentId, CurrentUserId);
        }



        // Shared method to check if user can delete content
        protected bool CanDelete(int contentId)
        {
            return _authService.CanDelete(contentId, CurrentUserId);
        }



        // Shared method to check if user can edit content
        protected bool CanEdit(int contentId)
        {
            return _authService.CanEdit(contentId, CurrentUserId);
        }



        // Shared method to get content by id (NOT USED AS OF NOW)
        protected Content? GetContent(int contentId)
        {
            return _contentService.GetContentById(contentId);
        }

    }
}
