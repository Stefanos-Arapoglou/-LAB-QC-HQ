using _LAB__QC_HQ.Interfaces;
using Microsoft.AspNetCore.Mvc;
using _LAB__QC_HQ.Models.ViewModels;

namespace _LAB__QC_HQ.Controllers
{
    public class KnowHowController : Controller
    {
        private readonly IKnowHowService _knowHowService;
        private readonly IContentService _contentService;

        public KnowHowController(
            IKnowHowService knowHowService,
            IContentService contentService)
        {
            _knowHowService = knowHowService;
            _contentService = contentService;
        }

/*        public IActionResult Details(int id)
        {
            var userId = User.GetUserId();

            if (!_contentService.CanUserViewContent(id, userId))
                return Forbid();

            *//*var knowHow = *//* load KnowHowDetail + Content *//*;*//*
            return View(knowHow);
        }*/

/*        [HttpPost]
        public IActionResult Create(CreateKnowHowViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var id = _knowHowService.CreateKnowHow(model, User.GetUserId());
            return RedirectToAction(nameof(Details), new { id });
        }*/
    }
}
