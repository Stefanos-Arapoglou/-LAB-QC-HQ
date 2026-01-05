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

                    var originalName = Path.GetFileName(input.FileUpload.FileName);
                    var sanitizedName = string.Concat(
                        originalName.Split(Path.GetInvalidFileNameChars())
                    );

                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);

                    var path = Path.Combine(uploads, sanitizedName);
                    int count = 1;

                    while (File.Exists(path))
                    {
                        var name = Path.GetFileNameWithoutExtension(sanitizedName);
                        var ext = Path.GetExtension(sanitizedName);
                        path = Path.Combine(uploads, $"{name}({count}){ext}");
                        count++;
                    }

                    using var stream = new FileStream(path, FileMode.Create);
                    await input.FileUpload.CopyToAsync(stream);

                    value = Path.GetFileName(path);
                }
                else
                {
                    value = input.ItemValue!;
                }

                _db.Items.Add(new Item
                {
                    ContentId = contentId,
                    ItemType = input.ItemType,
                    ItemValue = value
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Item?> GetItemByIdAsync(int itemId)
        {
            return await _db.Items.FindAsync(itemId);
        }

        public async Task<(byte[] data, string fileName)> GetFileAsync(int itemId)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null || item.ItemType != "File")
                throw new FileNotFoundException();

            var path = Path.Combine(_env.WebRootPath, "uploads", item.ItemValue);
            var data = await File.ReadAllBytesAsync(path);

            return (data, item.ItemValue);
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null)
                return;

            if (item.ItemType == "File")
            {
                var path = Path.Combine(_env.WebRootPath, "uploads", item.ItemValue);
                if (File.Exists(path))
                    File.Delete(path);
            }

            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}