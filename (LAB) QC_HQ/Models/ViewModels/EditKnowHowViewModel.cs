using _LAB__QC_HQ.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public class EditKnowHowViewModel : EditContentViewModel
    {
        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public byte RiskLevel { get; set; }

        [MinLength(1)]
        public List<EditItemInput> Items { get; set; } = new(); // ✅ Now uses EditItemInput
    }
}
