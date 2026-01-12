using _LAB__QC_HQ.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public class CreateKnowHowViewModel: EditContentViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public byte RiskLevel { get; set; }

        // Departments / clearance (required)
        [MinLength(1, ErrorMessage = "At least one department must be selected.")]
        public List<DepartmentClearanceInput> Departments { get; set; } = new();


    }
}
