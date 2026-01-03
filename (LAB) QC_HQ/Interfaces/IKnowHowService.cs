using _LAB__QC_HQ.Models.ViewModels;

namespace _LAB__QC_HQ.Interfaces
{
    public interface IKnowHowService
    {

        int CreateKnowHow(CreateKnowHowViewModel model, string userId);
    }
}
