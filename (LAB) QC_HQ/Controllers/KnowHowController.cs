/* NOTES 
 
This is a type specific Controller for Know-How content, inheriting from ContentController.

 
 
 */

using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using _LAB__QC_HQ.Models.Enums;

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
        // Directs to the know-how creation view. Populates the departments for selection, and sends specific view model
        // for know-how creation
        public IActionResult Create()
        {
            ViewBag.Departments = _contentService.GetAllDepartments();
            return View(new CreateKnowHowViewModel());
        }



        // POST: /KnowHow/Create
        // Handles the submission of the know-how creation form, and creates the know-how content
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

            // Shows the created know-how, by redirecting to Details action
            return RedirectToAction(nameof(Details), new { id = contentId });
        }



        // GET: /KnowHow/Details/5
        // Shows the details of a specific know-how content item
        public async Task<IActionResult> Details(int id)
        {
            //first check if the user can view this content
            if (!CanView(id))
                return Forbid();

            var knowHowDetail = await _knowHowService.GetKnowHowDetailAsync(id);
            if (knowHowDetail == null)
                return NotFound();

            return View("Details", knowHowDetail); 
        }



        // GET: /KnowHow
        // Lists all know-how content, including inactive ones
        public IActionResult Index()
        {
            var content = _contentService.GetTypeContentIncludingInactive(ContentType.KnowHow);
            return View(content);
        }



        // POST: /KnowHow/Delete/5
        // Deletes a specific know-how content item
        // AS OF NOW NOT NEEDED - HANDLED BY CONTENTBROWSER CONTROLLER // KEEPING FOR FUTURE REFERENCE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!CanDelete(id))
                return Forbid();
            try
            {
               
                await _contentService.DeleteContentAsync(id);

                TempData["SuccessMessage"] = "Know-How deleted successfully.";
                return RedirectToAction("Index", "ContentBrowse");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting Know-How: {ex.Message}";
                return RedirectToAction("Details", new { id });
            }
        }



        // POST: /KnowHow/Deactivate/5
        // Deactivates a specific know-how content item
        // AS OF NOW NOT NEEDED - HANDLED BY CONTENTBROWSER CONTROLLER // KEEPING FOR FUTURE REFERENCE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!CanEdit(id))
                return Forbid();

            await _contentService.DeactivateContentAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }



        // POST: /KnowHow/Activate/5
        // Activates a specific know-how content item
        // AS OF NOW NOT NEEDED - HANDLED BY CONTENTBROWSER CONTROLLER // KEEPING FOR FUTURE REFERENCE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            if (!CanEdit(id))
                return Forbid();

            await _contentService.ActivateContentAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }



        // GET: /KnowHow/Edit/5
        // Directs to the know-how editing view. Populates the departments for selection, and sends specific view model
        public async Task<IActionResult> Edit(int id)
        {
            //first check if the user can edit this content
            if (!CanEdit(id))
                return Forbid();

            //get the view model for editing
            var vm = await _knowHowService.GetEditViewModelAsync(id);
            if (vm == null)
                return NotFound();

            //populate departments for selection
            ViewBag.Departments = _contentService.GetAllDepartments();

            return View(vm);
        }



        // POST: /KnowHow/Edit/5
        // Handles the submission of the know-how editing form, and updates the know-how content
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditKnowHowViewModel model)
        {
            if (!CanEdit(id))
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _contentService.GetAllDepartments();
                return View(model);
            }

            await _knowHowService.UpdateKnowHowAsync(id, model);

            return RedirectToAction(nameof(Details), new { id });
        }


    }
}


