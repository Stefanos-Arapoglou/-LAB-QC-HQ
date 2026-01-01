using System.ComponentModel.DataAnnotations;

namespace _LAB__QC_HQ.Models.ViewModels
{
    public class CreateUserVM : EditUserVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImageFile { get; set; }
    }
}
