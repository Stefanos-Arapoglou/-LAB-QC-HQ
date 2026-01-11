using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class CreateItemInput: ItemInputBase
    {

        // ⭐ Drag/drop assigns this dynamically in UI
        public int DisplayOrder { get; set; }
    }
}
