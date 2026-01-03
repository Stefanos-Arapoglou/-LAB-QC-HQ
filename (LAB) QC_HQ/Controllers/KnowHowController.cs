using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _LAB__QC_HQ.Controllers
{
    public class KnowHowController : ContentController
    {
        private readonly IKnowHowService _knowHowService;

        public KnowHowController(
            IContentService contentService,
            IContentAuthorizationService authService,
            IKnowHowService knowHowService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IItemService itemService)
            : base(contentService, authService, userManager, env, itemService)
        {
            _knowHowService = knowHowService;
        }

        // GET: /KnowHow/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _contentService.GetAllDepartments();
            return View(new CreateKnowHowViewModel());
        }

        // POST: /KnowHow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKnowHowViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _contentService.GetAllDepartments();
                return View(model);
            }

            // Use CurrentUserId from ContentController
            var contentId = await _knowHowService.CreateKnowHowAsync(model, CurrentUserId);

            return RedirectToAction(nameof(Details), new { id = contentId });
        }

        // GET: /KnowHow/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!CanView(id))
                return Forbid();

            var knowHowDetail = await _knowHowService.GetKnowHowDetailAsync(id);
            if (knowHowDetail == null)
                return NotFound();

            return View("Details", knowHowDetail); // Ensure your view is Views/KnowHow/Details.cshtml
        }

        // GET: /KnowHow
        public IActionResult Index()
        {
            var content = _contentService.GetAllContent();
            return View(content);
        }
    }
}


