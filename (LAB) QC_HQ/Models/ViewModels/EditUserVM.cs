using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public class EditUserVM
    {
        [ValidateNever]
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; }

        public List<UserDepartmentEditVM> Departments { get; set; } = new();
    }
}
