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


        IEnumerable<Content> GetAllContent();
        Content? GetContentById(int contentId);
        KnowHowDetail? GetKnowHowDetail(int contentId);
        IEnumerable<Content> GetBrowsableContent();

        IEnumerable<Content> GetAllContentIncludingInactive();
        IEnumerable<Department> GetAllDepartments();

        IEnumerable<Content> GetTypeContentIncludingInactive(ContentType contentType);

        IEnumerable<Content> GetAllTypeContent(ContentType contentType);

        Task<ContentType> GetContentTypeAsync(int contentId);

        Task<bool> DeleteContentAsync(int contentId);

        Task UpdateContentAsync(int contentId, string title, IEnumerable<DepartmentClearanceInput> departments);

        Task ActivateContentAsync(int contentId);
        Task DeactivateContentAsync(int contentId);

    }
}
