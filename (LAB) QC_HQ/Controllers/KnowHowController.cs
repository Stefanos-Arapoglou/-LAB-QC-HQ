using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{
    public class KnowHowController : ContentController
    {
        private readonly IKnowHowService _knowHowService;

        public KnowHowController(
            IContentService contentService,
            IContentAuthorizationService authService,
            IKnowHowService knowHowService,
            UserManager<ApplicationUser> userManager)
            : base(contentService, authService, userManager)
        {
            _knowHowService = knowHowService;
        }

        // GET: /KnowHow
        public IActionResult Index()
        {
            var content = _contentService
                .GetAllContent()
                .Where(c => CanView(c.ContentId))
                .ToList();

            return View(content);
        }

        // GET: /KnowHow/Create
        public IActionResult Create()
        {
            return View(new CreateKnowHowViewModel());
        }

        // POST: /KnowHow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateKnowHowViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contentId = _knowHowService.CreateKnowHow(model, CurrentUserId);

            return RedirectToAction(nameof(Details), new { id = contentId });
        }

        // GET: /KnowHow/Details/5
        public override IActionResult Details(int id)
        {
            if (!CanView(id))
                return Forbid();

            var knowHowDetail = _contentService.GetKnowHowDetail(id);
            if (knowHowDetail == null)
                return NotFound();

            return View("KnowHowDetails", knowHowDetail);
        }
    }
}

