namespace _LAB__QC_HQ.Models.ViewModels
{
    public class ContentBrowseViewModel
    {
        public int ContentId { get; set; }
        public string Title { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string CreatedByUserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public List<DepartmentClearanceViewModel> Departments { get; set; } = new();
    }
}
