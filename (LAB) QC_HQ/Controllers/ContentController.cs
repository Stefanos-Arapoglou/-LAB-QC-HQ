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

        protected string CurrentUserId =>
    _userManager.GetUserId(User)!;

        // Shared method to check if user can view content
        protected bool CanView(int contentId)
        {
            return _authService.CanView(contentId, CurrentUserId);
        }

        // Shared method to get content by id
        protected Content? GetContent(int contentId)
        {
            return _contentService.GetContentById(contentId);
        }

        protected bool CanDelete(int contentId)
        {
            return _authService.CanDelete(contentId, CurrentUserId);
        }

        protected bool CanEdit(int contentId)
        {
            return _authService.CanEdit(contentId, CurrentUserId);
        }

    }
}
