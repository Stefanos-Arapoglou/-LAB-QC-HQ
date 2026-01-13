/* NOTES 
 
This ViewModel is used by other ViewModels and parts of the code, specifically to connect users with Deprartments and their clearance levels.
It is used by:

EditUserVM
- CreateUserVM
- Edit Action of UserController
- Create Action of UserController
 
 */

using _LAB__QC_HQ.MetaData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace _LAB__QC_HQ.Models.ViewModels
{
    [ModelMetadataType(typeof(UserDepartmentEditMetadata))]
    public class UserDepartmentEditVM
    {
        public int DepartmentId { get; set; }

        [BindNever]
        [ValidateNever]
        public string DepartmentName { get; set; }

        public bool IsSelected { get; set; }

        public byte ClearanceLevel { get; set; }
    }
}
