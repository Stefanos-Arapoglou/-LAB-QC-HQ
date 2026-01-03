using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;

namespace _LAB__QC_HQ.Services
{
    public class ContentAuthorizationService : IContentAuthorizationService
    {
        private readonly ApplicationDbContext _db;

        public ContentAuthorizationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CanView(int contentId, string userId)
        {
            return _db.ContentDepartments.Any(cd =>
                cd.ContentId == contentId &&
                _db.UserDepartments.Any(ud =>
                    ud.UserId == userId &&
                    ud.DepartmentId == cd.DepartmentId &&
                    ud.ClearanceLevel >= cd.ClearanceLevelRequired
                )
            );
        }
    }
}
