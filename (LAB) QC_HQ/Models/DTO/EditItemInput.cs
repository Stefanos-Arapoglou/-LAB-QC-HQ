/*NOTES
 
In reality, this is the only ItemInput model that i use everywhere. 
I did not want to simplify into a single DTO in case I need to diverge Create and Edit in the future.
 
 */

using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class EditItemInput: ItemInputBase
    {
        public EditItemInput() { }
        public int ItemId { get; set; }

    }
}
