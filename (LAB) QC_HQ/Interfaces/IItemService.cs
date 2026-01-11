using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;

namespace _LAB__QC_HQ.Interfaces
{
    public interface IItemService
    {
        Task AddItemsAsync(int contentId, IEnumerable<CreateItemInput> items);
        Task<Item?> GetItemByIdAsync(int itemId);
        Task<(byte[] data, string fileName)> GetFileAsync(int itemId);
        Task DeleteItemAsync(int itemId);

        Task UpdateItemsAsync(int contentId, List<CreateItemInput> items);
    }
}
