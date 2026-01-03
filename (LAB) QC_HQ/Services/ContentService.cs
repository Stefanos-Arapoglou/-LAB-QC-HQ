using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;
using _LAB__QC_HQ.Models.Enums;

namespace _LAB__QC_HQ.Services
{
    public class ContentService : IContentService
    {
        private readonly ApplicationDbContext _db;

        public ContentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Content CreateContent(
            string title,
            ContentType contentType,
            string userId,
            IEnumerable<DepartmentClearanceInput> departments)
        {
            if (departments == null || !departments.Any())
                throw new InvalidOperationException(
                    "Content must be assigned to at least one department.");

            var content = new Content
            {
                Title = title,
                ContentType = contentType.ToString(),
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                TimesViewed = 0
            };

            _db.Contents.Add(content);
            _db.SaveChanges();

            foreach (var dep in departments)
            {
                _db.ContentDepartments.Add(new ContentDepartment
                {
                    ContentId = content.ContentId,
                    DepartmentId = dep.DepartmentId,
                    ClearanceLevelRequired = dep.ClearanceLevelRequired
                });
            }

            _db.SaveChanges();
            return content;
        }

        public bool CanUserViewContent(int contentId, string userId)
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
