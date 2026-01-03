using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;

namespace _LAB__QC_HQ.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ItemService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task AddItemsAsync(int contentId, IEnumerable<CreateItemInput> items)
        {
            foreach (var input in items)
            {
                string value;

                if (input.ItemType == "File")
                {
                    if (input.FileUpload == null)
                        throw new InvalidOperationException("File item requires upload.");

                    // Clean filename to remove any path info
                    var originalName = Path.GetFileName(input.FileUpload.FileName);

                    // Sanitize: remove invalid characters
                    var sanitizedFileName = string.Concat(originalName.Split(Path.GetInvalidFileNameChars()));

                    // Ensure uploads folder exists
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);

                    // Build initial path
                    var path = Path.Combine(uploads, sanitizedFileName);

                    // Prevent overwriting existing files
                    int count = 1;
                    while (System.IO.File.Exists(path))
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                        var ext = Path.GetExtension(sanitizedFileName);
                        path = Path.Combine(uploads, $"{fileNameWithoutExt}({count}){ext}");
                        count++;
                    }

                    // Save file to disk
                    using var stream = new FileStream(path, FileMode.Create);
                    await input.FileUpload.CopyToAsync(stream);

                    // Store the final file name in DB
                    value = Path.GetFileName(path);
                }
                else
                {
                    // Text or Link items
                    value = input.ItemValue!;
                }

                // Add to DB
                _db.Items.Add(new Item
                {
                    ContentId = contentId,
                    ItemType = input.ItemType,
                    ItemValue = value
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Item?> GetItemAsync(int itemId)
        {
            return await _db.Items.FindAsync(itemId);
        }

        public async Task<(byte[] data, string fileName)> GetFileAsync(int itemId)
        {
            var item = await _db.Items.FindAsync(itemId);
            if (item == null || item.ItemType != "File")
                throw new FileNotFoundException();

            var path = Path.Combine(_env.WebRootPath, "uploads", item.ItemValue);
            var data = await File.ReadAllBytesAsync(path);

            return (data, item.ItemValue);
        }
    }
}
