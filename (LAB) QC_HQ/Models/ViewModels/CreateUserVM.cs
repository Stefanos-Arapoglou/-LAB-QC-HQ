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