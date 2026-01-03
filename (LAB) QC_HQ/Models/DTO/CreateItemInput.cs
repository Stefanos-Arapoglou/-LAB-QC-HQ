using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class CreateItemInput
    {
        [Required]
        public string ItemType { get; set; } = "Text"; // Text / Link / File

        [StringLength(5000)]
        public string? ItemValue { get; set; }

        // For files
        public IFormFile? FileUpload { get; set; }
    }
}
