/* NOTES 

As of now, this controller is not being used! It could be completely commented out.
But in the future, in case of specific Item-related actions, that are NOT connected to Content directly, will go through here

Non Content binded item creation, updating, binding and other actions will be added here in the future

 */


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

        // GET: /Item/DownloadItem/5
        // Downloads the file associated with the item
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



        // POST: /Item/UpdateItems/5
        // Updates multiple items associated with a content (WILL BE REMOVED)
        [HttpPost]
        public async Task<IActionResult> UpdateItems(int contentId, List<EditItemInput> items)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _itemService.UpdateItemsAsync(contentId, items);
            return Ok();
        }

    }
}
