/* NOTES 
 
Contains all the Logic for content authorization, based on user roles and department clearance levels.
    1) CanView
    2) CanEdit
    3) CanDelete
    4) isAdmin (private helper method)
 
 */

using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _LAB__QC_HQ.Services
{
    public class ContentAuthorizationService : IContentAuthorizationService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContentAuthorizationService(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Check if user can view the content based on clearance level
        public bool CanView(int contentId, string userId)
        {

            /*// Admin can view everything
            if (IsUserInRole(userId, "Admin"))
                return true;*/

            return _db.ContentDepartments.Any(cd =>
                cd.ContentId == contentId &&
                _db.UserDepartments.Any(ud =>
                    ud.UserId == userId &&
                    ud.DepartmentId == cd.DepartmentId &&
                    ud.ClearanceLevel >= cd.ClearanceLevelRequired
                )
            );
        }



        // Check if user can edit the content (only Admin can edit as of now)
        public bool CanEdit(int contentId, string userId)
        {
            // Only Admin can edit
            return IsAdmin(userId);
        }



        // Check if user can delete the content (only Admin can delete as of now)
        public bool CanDelete(int contentId, string userId)
        {
            // Only Admin can delete
            return IsAdmin(userId);
        }



        // Check if user can create content (Not Implemented as of now)
        public bool CanCreate(string userId)
        {
            // Only Admin can create
            return IsAdmin(userId);
        }



        // Private helper method to check if user is in Admin role
        private bool IsAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
                return false;

            return _userManager.IsInRoleAsync(user, "Admin").Result;
        }

    }
}
