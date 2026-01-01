namespace _LAB__QC_HQ.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;



public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public DateTime? HireDate { get; set; }
    public byte[] ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property for roles (optional but useful)
    //public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    // Helper method to get role names
    [NotMapped] // This property won't be stored in database
    public List<string> RoleNames { get; set; } = new List<string>();

    public virtual ICollection<Content> Contents { get; set; }
    public virtual ICollection<KnowHowUser> KnowHowUsers { get; set; }
    public virtual ICollection<UserDepartment> UserDepartments { get; set; }
}

