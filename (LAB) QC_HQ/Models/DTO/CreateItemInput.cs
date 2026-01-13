/* NOTES 

As of now, CreateItemInput is NOT used anywhere in the codebase.
As the app grows and gets more complex, having separate DTOs for creating and editing items might be beneficial.
Main difference between Creation and Edit is the need for ItemId during Edit operations.

 */

using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.DTO
{
    public class CreateItemInput: ItemInputBase
    {

    }
}
