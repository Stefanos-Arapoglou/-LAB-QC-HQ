using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{
    public class ItemController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IWebHostEnvironment _env;
        private readonly IItemService _itemService;

        public ItemController(IContentService contentService, IWebHostEnvironment env, IItemService itemService)
        {
            _contentService = contentService;
            _env = env;
            _itemService = itemService;
        }

        public async Task<IActionResult> DownloadItem(int id)
        {
            try
            {
                var (data, fileName) = await _itemService.GetFileAsync(id);

                // You can improve MIME detection later if needed
                var contentType = "application/octet-stream";

                return File(data, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateItems(int contentId, List<CreateItemInput> items)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _itemService.UpdateItemsAsync(contentId, items);
            return Ok();
        }

    }
}
