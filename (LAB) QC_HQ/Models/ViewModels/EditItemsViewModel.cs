using _LAB__QC_HQ.Models.DTO;

public class EditItemsViewModel
{
    public int ContentId { get; set; }
    public List<EditItemInput> Items { get; set; } = new List<EditItemInput>();
}
