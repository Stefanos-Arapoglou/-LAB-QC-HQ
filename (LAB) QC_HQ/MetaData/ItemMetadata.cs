/*NOTES

In reality, EFCore does NOT understand the [column] of a metadata. 
I am keeping this Metadata for future reference, in case I need to add attributes, but as of now it is useless

 */

using System.ComponentModel.DataAnnotations.Schema;

namespace _LAB__QC_HQ.MetaData
{
    public class ItemMetadata
    {
        [Column("item_title")]
        public string ItemTitle { get; set; } = null!;
    }
}
