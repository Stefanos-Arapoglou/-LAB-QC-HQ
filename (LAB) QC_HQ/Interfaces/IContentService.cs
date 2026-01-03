using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.Enums;
using _LAB__QC_HQ.Models.DTO;

namespace _LAB__QC_HQ.Interfaces
{
    public interface IContentService
    {
        Content CreateContent(
            string title,
            ContentType contentType,
            string userId,
            IEnumerable<DepartmentClearanceInput> departments);

        bool CanUserViewContent(int contentId, string userId);
    }
}
