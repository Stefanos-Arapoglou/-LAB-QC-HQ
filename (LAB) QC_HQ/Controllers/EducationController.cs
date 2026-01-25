using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{

    public class EducationController : ContentController
    {
        private readonly IEducationService _educationService;

        public EducationController(
            IContentService contentService,
            IContentAuthorizationService authService,
            IEducationService educationService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IItemService itemService)
            : base(contentService, authService, userManager, env, itemService)
        {
            _educationService = educationService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TestView()
        {
            return View();

        }

        public IActionResult Create()
        {
            ViewBag.Departments=_contentService.GetAllDepartments();
            return View(new CreateEducationViewModel());
        }
    }
}
