/* NOTES 
 
A DTO that is used everywhere, where the connection between anything (Content/Item) and the Clearance level of a Department is needed.
 
 */

using System.ComponentModel.DataAnnotations;
namespace _LAB__QC_HQ.Models.DTO
{
    public class DepartmentClearanceInput
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public byte ClearanceLevelRequired { get; set; }
    }
}
