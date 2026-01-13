/* NOTES 
 
This is an abstract base ViewModel for editing content, containing common properties shared across different content types.
Each type (if needed) inherits from this base ViewModel and adds the type-specific properties.

Interface IContentEditBase is implemented to ensure consistency across different content edit ViewModels.

 */

using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public abstract class EditContentViewModel: IContentEditBase

    {
        [Required]
        public int ContentId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;


        [MinLength(1, ErrorMessage = "At least one department must be selected.")]
        public List<DepartmentClearanceInput> Departments { get; set; } = new();

        // Add Items property here
        public List<EditItemInput> Items { get; set; } = new();
    }
}