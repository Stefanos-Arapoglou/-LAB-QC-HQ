/* NOTES 
 
Shares similar structure to EditUserVM but includes additional fields required for user creation, Password and ProfileImageFile
Its seems counterintuitive to have Email as 'new' here, but as of ChatGPT guidance this is the good practice in this case.
This solution came up in multiple prompts / discussions 
 
 */

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public class CreateUserVM : EditUserVM
    {
        [Required]
        [EmailAddress]
        public new string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImageFile { get; set; }
    }
}