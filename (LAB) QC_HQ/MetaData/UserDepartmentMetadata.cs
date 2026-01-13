/*NOTES

In reality, EFCore does NOT understand the [column] of a metadata. 
I am keeping this Metadata for future reference, in case I need to add attributes, but as of now it is useless

 */

namespace _LAB__QC_HQ.MetaData
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserDepartmentMetadata
    {
        [Column("user_id")]
        public string UserId { get; set; }

        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Column("Clearance_Level")]
        public byte ClearanceLevel { get; set; }
    }
}