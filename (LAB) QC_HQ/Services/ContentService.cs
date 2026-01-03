using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;
using _LAB__QC_HQ.Models.Enums;
using _LAB__QC_HQ.Models.Maps;
using Microsoft.EntityFrameworkCore;

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
                ContentType = contentType.ToDbString(),
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


        public IEnumerable<Content> GetBrowsableContent()
        {
            return _db.Contents
                .Where(c => c.IsActive)
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.ContentDepartments)
                    .ThenInclude(cd => cd.Department)
                .ToList();
        }

        public IEnumerable<Content> GetAllContent()
        {
            return _db.Contents.Where(c => c.IsActive).ToList();
        }

        public Content? GetContentById(int contentId)
        {
            return _db.Contents.FirstOrDefault(c => c.ContentId == contentId && c.IsActive);
        }

        public KnowHowDetail? GetKnowHowDetail(int contentId)
        {
            return _db.KnowHowDetails
                .Include(k => k.Content)                     // Include the Content entity
                    .ThenInclude(c => c.ContentDepartments) // Include Departments
                        .ThenInclude(cd => cd.Department)
                .Include(k => k.Content)
                    .ThenInclude(c => c.Items)              // Include Items
                .Include(k => k.Content)
                    .ThenInclude(c => c.CreatedByNavigation)// Include CreatedBy navigation
                .FirstOrDefault(k => k.ContentId == contentId && k.IsActive);
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            return _db.Departments.OrderBy(d => d.Name).ToList();
        }

        public Item? GetItemById(int itemId)
        {
            return _db.Items.FirstOrDefault(i => i.ItemId == itemId);
        }

        public void AddItemToContent(int contentId, string itemType, string itemValue)
        {
            var item = new Item
            {
                ContentId = contentId,
                ItemType = itemType,
                ItemValue = itemValue
            };
            _db.Items.Add(item);
            _db.SaveChanges();
        }

    }

}
