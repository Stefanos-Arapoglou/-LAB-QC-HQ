namespace _LAB__QC_HQ.Models.DTO
{
    public class ItemInputBase
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ItemTitle { get; set; } = string.Empty;
        public string? ItemValue { get; set; }
        public int DisplayOrder { get; set; }
        public IFormFile? FileUpload { get; set; }
    }
}
