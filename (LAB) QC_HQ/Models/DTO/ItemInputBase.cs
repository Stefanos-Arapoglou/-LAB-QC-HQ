/* NOTES 

Keeping a base model that does NOT contain ItemId, to be used for both Create and Edit operations.

 */


using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class ItemInputBase
    {
        /*        public int ItemId { get; set; }*/
        [Required]
        public string ItemType { get; set; } = "Text";

        [Required]
        public string ItemTitle { get; set; } = null!;

        [StringLength(5000)]
        public string? ItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public IFormFile? FileUpload { get; set; }
    }
}
