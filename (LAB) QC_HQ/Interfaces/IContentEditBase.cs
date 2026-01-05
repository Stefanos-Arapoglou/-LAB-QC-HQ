using _LAB__QC_HQ.Models.DTO;

namespace _LAB__QC_HQ.Interfaces
{
    public interface IContentEditBase
    {
        string Title { get; set; }
        List<DepartmentClearanceInput> Departments { get; set; }
    }
}
