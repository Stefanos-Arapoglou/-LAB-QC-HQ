/* NOTES 
 
Cream and butter of item management, including adding, updating, deleting items
Includes handling file uploads and downloads associated with items.

SUMMARY:
    1) GET METHODS
        - GetItemByIdAsync
    2) ADD / UPDATE / DELETE METHODS
        - AddItemsAsync
        - UpdateItemsAsync
        - DeleteItemAsync
    3) FILE HANDLING METHODS
        - GetFileAsync
        - SaveUploadAsync (private helper)
        - DeleteItemFile (private helper)
 
 */

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


        //~~~~~~~~~~~~~~~~~~ GET METHODS ~~~~~~~~~~~~~~~~~~


        // Get item by ID
        public async Task<Item?> GetItemByIdAsync(int itemId)
        {
            return await _db.Items.FindAsync(itemId);
        }





        //~~~~~~~~~~~~~~~~~~ ADD / UPDATE / DELETE METHODS ~~~~~~~~~~~~~~~~~~


        // Add multiple items
        public async Task AddItemsAsync(int contentId, IEnumerable<EditItemInput> items)
        {
            foreach (var input in items)
            {
                string value = input.ItemValue ?? "";

                if ((input.ItemType == "File" || input.ItemType == "Image") && input.FileUpload != null)
                {
                    value = await SaveUploadAsync(input.FileUpload, input.ItemType);
                }

                _db.Items.Add(new Item
                {
                    ContentId = contentId,
                    ItemType = input.ItemType,
                    ItemTitle = input.ItemTitle,
                    ItemValue = value,
                    DisplayOrder = input.DisplayOrder
                });
            }

            await _db.SaveChangesAsync();
        }



        // Update multiple items
        public async Task UpdateItemsAsync(int contentId, List<EditItemInput> items)
        {
            var existingItems = await _db.Items
                .Where(i => i.ContentId == contentId)
                .ToListAsync();

            foreach (var input in items)
            {
                if (input.ItemId > 0)
                {
                    var item = existingItems.FirstOrDefault(x => x.ItemId == input.ItemId);
                    if (item == null)
                        continue;

                    item.ItemTitle = input.ItemTitle;
                    item.ItemType = input.ItemType;
                    item.DisplayOrder = input.DisplayOrder;

                    if ((input.ItemType == "File" || input.ItemType == "Image")
                        && input.FileUpload != null)
                    {
                        // 🔹 delete old file BEFORE replacing
                        DeleteItemFile(item);

                        item.ItemValue = await SaveUploadAsync(
                            input.FileUpload,
                            input.ItemType
                        );
                    }
                    else
                    {
                        item.ItemValue = input.ItemValue ?? "";
                    }
                }
                else
                {
                    // NEW ITEM
                    string value = input.ItemValue ?? "";

                    if ((input.ItemType == "File" || input.ItemType == "Image")
                        && input.FileUpload != null)
                    {
                        value = await SaveUploadAsync(input.FileUpload, input.ItemType);
                    }

                    _db.Items.Add(new Item
                    {
                        ContentId = contentId,
                        ItemTitle = input.ItemTitle,
                        ItemType = input.ItemType,
                        ItemValue = value,
                        DisplayOrder = input.DisplayOrder
                    });
                }
            }

            // Delete removed items
            var incomingIds = items.Where(i => i.ItemId > 0)
                                   .Select(i => i.ItemId)
                                   .ToList();

            var toDelete = existingItems
                .Where(x => !incomingIds.Contains(x.ItemId));

            foreach (var item in toDelete)
            {
                DeleteItemFile(item);
                _db.Items.Remove(item);
            }

            await _db.SaveChangesAsync();
        }



        // Delete item by ID
        public async Task DeleteItemAsync(int itemId)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null)
                return;

            if (item.ItemType == "File" || item.ItemType == "Image")
            {
                var folder = item.ItemType == "Image" ? "uploads/images" : "uploads";
                var path = Path.Combine(_env.WebRootPath, folder, item.ItemValue);

                if (File.Exists(path))
                    File.Delete(path);
            }

            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }





        // ~~~~~~~~~~~~~~~~~~ FILE HANDLING METHODS ~~~~~~~~~~~~~~~~~~


        // Get file data and filename for download
        public async Task<(byte[] data, string fileName)> GetFileAsync(int itemId)
        {
            var item = await GetItemByIdAsync(itemId);
            if (item == null || (item.ItemType != "File" && item.ItemType != "Image"))
                throw new FileNotFoundException();

            var folder = item.ItemType == "Image" ? "uploads/images" : "uploads";
            var path = Path.Combine(_env.WebRootPath, folder, item.ItemValue);

            var data = await File.ReadAllBytesAsync(path);
            return (data, item.ItemValue);
        }



        // Helper Method: Save uploaded file and return stored filename
        private async Task<string> SaveUploadAsync(IFormFile fileUpload, string itemType)
        {
            var sanitized = Path.GetFileName(fileUpload.FileName);

            // Image files stored separately
            var folder = itemType == "Image"
                ? Path.Combine(_env.WebRootPath, "uploads/images")
                : Path.Combine(_env.WebRootPath, "uploads");

            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, sanitized);

            int count = 1;
            while (System.IO.File.Exists(path))
            {
                var name = Path.GetFileNameWithoutExtension(sanitized);
                var ext = Path.GetExtension(sanitized);
                path = Path.Combine(folder, $"{name}({count}){ext}");
                count++;
            }

            using var stream = new FileStream(path, FileMode.Create);
            await fileUpload.CopyToAsync(stream);

            // store just filename
            return Path.GetFileName(path);
        }



        // Helper Method: Delete the file associated with an item
        private void DeleteItemFile(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.ItemValue))
                return;

            if (item.ItemType != "File" && item.ItemType != "Image")
                return;

            var folder = item.ItemType == "Image" ? "uploads/images" : "uploads";
            var path = Path.Combine(_env.WebRootPath, folder, item.ItemValue);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
