namespace _LAB__QC_HQ.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        // Optional: Add a display property
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
