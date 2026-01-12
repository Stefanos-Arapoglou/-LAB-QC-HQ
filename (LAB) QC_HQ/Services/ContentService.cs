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
                .Include(c => c.Items)
                .ToList();
        }

        public IEnumerable<Content> GetAllContentIncludingInactive()
        {
            return _db.Contents
                .Include(c => c.Items)
                .Include(c => c.ContentDepartments)
                    .ThenInclude(cd => cd.Department)
                .Include(c => c.CreatedByNavigation)
                .ToList();
        }

        public IEnumerable<Content> GetAllContent()
        {
            return _db.Contents.Where(c => c.IsActive).ToList();
        }

        public IEnumerable<Content> GetTypeContentIncludingInactive(ContentType contentType)
        {
            return _db.Contents
                .Where(c => c.ContentType == contentType.ToDbString())
                .Include(c => c.Items)
                .Include(c => c.ContentDepartments)
                    .ThenInclude(cd => cd.Department)
                .Include(c => c.CreatedByNavigation)
                .ToList();
        }

        public IEnumerable<Content> GetAllTypeContent(ContentType contentType)
        {
            return _db.Contents
                .Where(c => c.IsActive)
                .Where(c => c.ContentType == contentType.ToDbString())
                .ToList();
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

        public async Task<ContentType> GetContentTypeAsync(int contentId)
        {
            // Get the string value from database (exactly as stored by ToDbString())
            var contentTypeString = await _db.Contents
                .Where(c => c.ContentId == contentId)
                .Select(c => c.ContentType) // This returns string like "Know-How", "Educational", etc.
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(contentTypeString))
                throw new KeyNotFoundException($"Content with ID {contentId} not found");

            // Convert the database string back to ContentType enum
            // Must match EXACTLY what ToDbString() produces
            return contentTypeString switch
            {
                "Know-How" => ContentType.KnowHow,
                "Educational" => ContentType.Educational,
                "Announcement" => ContentType.Announcement,
                "File" => ContentType.File,
                _ => throw new InvalidOperationException($"Unknown content type string: {contentTypeString}")
            };
        }

        // Add to ContentService class
        public async Task<bool> DeleteContentAsync(int contentId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1. Get content type first
                var contentType = await GetContentTypeAsync(contentId);

                // 2. Delete type-specific records
                switch (contentType)
                {
                    case ContentType.KnowHow:
                        // Delete KnowHowDetails
                        var knowHowDetails = await _db.KnowHowDetails
                            .Where(k => k.ContentId == contentId)
                            .ToListAsync();
                        _db.KnowHowDetails.RemoveRange(knowHowDetails);
                        break;

                    case ContentType.Educational:
                        // Delete Educational-specific records
                        break;

                        // Add other content types as needed
                }

                // 3. Delete related items
                var items = await _db.Items
                    .Where(i => i.ContentId == contentId)
                    .ToListAsync();
                _db.Items.RemoveRange(items);

                // 4. Delete department associations
                var departments = await _db.ContentDepartments
                    .Where(cd => cd.ContentId == contentId)
                    .ToListAsync();
                _db.ContentDepartments.RemoveRange(departments);

                // 5. Finally delete the Content record (HARD DELETE)
                var content = await _db.Contents.FindAsync(contentId);
                if (content != null)
                {
                    _db.Contents.Remove(content); // This is the hard delete
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateContentAsync(
            int contentId,
            string title,
            IEnumerable<DepartmentClearanceInput> departments)
        {
            var content = await _db.Contents
                .Include(c => c.ContentDepartments)
                .FirstOrDefaultAsync(c => c.ContentId == contentId);

            if (content == null)
                throw new KeyNotFoundException($"Content {contentId} not found.");

            // 1️⃣ Update scalar fields
            content.Title = title.Trim();

            // 2️⃣ Validate departments
            var departmentList = departments
                .Where(d => d.DepartmentId > 0)
                .ToList();

            if (!departmentList.Any())
                throw new InvalidOperationException(
                    "At least one department must be assigned.");

            // ❗ Enforce uniqueness
            if (departmentList
                .GroupBy(d => d.DepartmentId)
                .Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException(
                    "Each department can only be assigned once.");
            }

            // 3️⃣ Replace department assignments (safe way)
            _db.ContentDepartments.RemoveRange(content.ContentDepartments);

            foreach (var dep in departmentList)
            {
                _db.ContentDepartments.Add(new ContentDepartment
                {
                    ContentId = contentId,
                    DepartmentId = dep.DepartmentId,
                    ClearanceLevelRequired = dep.ClearanceLevelRequired
                });
            }

            await _db.SaveChangesAsync();
        }


        public async Task ActivateContentAsync(int contentId)
        {
            var content = await _db.Contents.FindAsync(contentId);
            if (content == null)
                throw new KeyNotFoundException();

            content.IsActive = true;
            await _db.SaveChangesAsync();
        }

        public async Task DeactivateContentAsync(int contentId)
        {
            var content = await _db.Contents.FindAsync(contentId);
            if (content == null)
                throw new KeyNotFoundException();

            content.IsActive = false;
            await _db.SaveChangesAsync();
        }


    }
    }





