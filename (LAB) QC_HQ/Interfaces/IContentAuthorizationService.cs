namespace _LAB__QC_HQ.Interfaces
{
    public interface IContentAuthorizationService
    {
        bool CanView(int contentId, string userId);
        bool CanDelete(int contentId, string userId);
        bool CanEdit(int contentId, string userId);
    }
}
