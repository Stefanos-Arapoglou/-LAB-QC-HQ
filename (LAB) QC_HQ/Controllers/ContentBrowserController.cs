using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            var content = _contentService
                .GetBrowsableContent()
                .Where(c => _authService.CanView(c.ContentId, CurrentUserId))
                .ToList();

            return View(content);
        }
    }
}
