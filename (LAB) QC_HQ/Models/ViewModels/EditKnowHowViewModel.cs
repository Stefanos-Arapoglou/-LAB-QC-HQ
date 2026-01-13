/* NOTES 
 
Type-Specific ViewModel for editing Know-How content, inheriting from EditContentViewModel to reuse common content properties.

 */

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


    }
}
