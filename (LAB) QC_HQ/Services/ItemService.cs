using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;
using Microsoft.EntityFrameworkCore;

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
                    ItemTitle = input.ItemTitle,
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


        // Updates, adds, and removes items for a given content
        public async Task UpdateItemsAsync(int contentId, List<EditItemInput> items)
        {
            var existingItems = await _db.Items
                .Where(i => i.ContentId == contentId)
                .ToListAsync();

            foreach (var input in items)
            {
                if (input.ItemId > 0)
                {
                    // Update existing
                    var item = existingItems.FirstOrDefault(x => x.ItemId == input.ItemId);
                    if (item != null)
                    {
                        item.ItemTitle = input.ItemTitle;
                        item.ItemType = input.ItemType;

                        if (input.ItemType == "File" && input.FileUpload != null)
                        {
                            // Save new file
                            var uploads = Path.Combine(_env.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploads);

                            var sanitizedFileName = Path.GetFileName(input.FileUpload.FileName);
                            var path = Path.Combine(uploads, sanitizedFileName);
                            int count = 1;
                            while (System.IO.File.Exists(path))
                            {
                                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                                var ext = Path.GetExtension(sanitizedFileName);
                                path = Path.Combine(uploads, $"{fileNameWithoutExt}({count}){ext}");
                                count++;
                            }

                            using var stream = new FileStream(path, FileMode.Create);
                            await input.FileUpload.CopyToAsync(stream);
                            item.ItemValue = Path.GetFileName(path);
                        }
                        else
                        {
                            item.ItemValue = input.ItemValue;
                        }
                    }
                }
                else
                {
                    // Add new
                    string value = input.ItemValue ?? "";
                    if (input.ItemType == "File" && input.FileUpload != null)
                    {
                        var uploads = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploads);

                        var sanitizedFileName = Path.GetFileName(input.FileUpload.FileName);
                        var path = Path.Combine(uploads, sanitizedFileName);
                        int count = 1;
                        while (System.IO.File.Exists(path))
                        {
                            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                            var ext = Path.GetExtension(sanitizedFileName);
                            path = Path.Combine(uploads, $"{fileNameWithoutExt}({count}){ext}");
                            count++;
                        }

                        using var stream = new FileStream(path, FileMode.Create);
                        await input.FileUpload.CopyToAsync(stream);
                        value = Path.GetFileName(path);
                    }

                    _db.Items.Add(new Item
                    {
                        ContentId = contentId,
                        ItemTitle = input.ItemTitle,
                        ItemType = input.ItemType,
                        ItemValue = value
                    });
                }
            }

            // Remove deleted items
            var incomingIds = items.Where(i => i.ItemId > 0).Select(i => i.ItemId).ToList();
            var toDelete = existingItems.Where(x => !incomingIds.Contains(x.ItemId)).ToList();
            if (toDelete.Any())
            {
                _db.Items.RemoveRange(toDelete);
            }

            await _db.SaveChangesAsync();
        }

    }
}