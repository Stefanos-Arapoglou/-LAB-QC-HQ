using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;

namespace _LAB__QC_HQ.Interfaces
{
    public interface IKnowHowService
    {
        // Create KnowHow content
        Task<int> CreateKnowHowAsync(CreateKnowHowViewModel model, string userId);

        // Get a KnowHowDetail by contentId
        Task<KnowHowDetail?> GetKnowHowDetailAsync(int contentId);

        Task UpdateKnowHowAsync(int contentId, EditKnowHowViewModel model);

        Task<EditKnowHowViewModel?> GetEditViewModelAsync(int contentId);

    }
}
