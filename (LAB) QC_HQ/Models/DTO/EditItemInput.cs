using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class EditItemInput: ItemInputBase
    {
        public EditItemInput() { }
        public int ItemId { get; set; } // ✅ Needed for updates

        [Required]
        public string ItemType { get; set; } = "Text"; // Text / Link / File

        [Required]
        public string ItemTitle { get; set; } = null!; // Title / Description

        [StringLength(5000)]
        public string? ItemValue { get; set; }

        // For file uploads
        public IFormFile? FileUpload { get; set; }

        // ⭐ Needed so reordered list posts back correctly
        public int DisplayOrder { get; set; }
    }
}
