using _LAB__QC_HQ.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace _LAB__QC_HQ.Controllers
{
    public class ItemController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IWebHostEnvironment _env;

        public ItemController(IContentService contentService, IWebHostEnvironment env)
        {
            _contentService = contentService;
            _env = env;
        }

        // GET: /Item/DownloadItem/5
        public IActionResult DownloadItem(int id)
        {
            // Get the item from the database
            var item = _contentService.GetItemById(id);
            if (item == null || item.ItemType != "File")
                return NotFound();

            // Build the physical file path (adjust folder as needed)
            var filePath = Path.Combine(_env.WebRootPath, "uploads", item.ItemValue);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Determine MIME type (optional: improve with real MIME detection)
            var mimeType = "application/octet-stream";

            // Return the file
            return PhysicalFile(filePath, mimeType, item.ItemValue);
        }
    }
}
