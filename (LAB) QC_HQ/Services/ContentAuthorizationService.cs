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


        public bool CanEdit(int contentId, string userId)
        {
            // Only Admin can edit
            return IsAdmin(userId);
        }

        public bool CanDelete(int contentId, string userId)
        {
            // Only Admin can delete
            return IsAdmin(userId);
        }

/*        public bool CanCreate(string userId)
        {
            // Only Admin can create
            return IsAdmin(userId);
        }*/

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
