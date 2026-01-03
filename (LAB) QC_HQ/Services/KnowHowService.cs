using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.Enums;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace _LAB__QC_HQ.Services
{
    public class KnowHowService : IKnowHowService
    {
        private readonly ApplicationDbContext _db;
        private readonly IContentService _contentService;
        private readonly IItemService _itemService; // inject item service


        public KnowHowService(
            ApplicationDbContext db,
            IContentService contentService,
            IItemService itemService
    )
        {
            _db = db;
            _contentService = contentService;
            _itemService = itemService;

        }

        public async Task<int> CreateKnowHowAsync(CreateKnowHowViewModel model, string userId)
        {
            // 1️⃣ Create Content
            var content = _contentService.CreateContent(
                model.Title,
                ContentType.KnowHow,
                userId,
                model.Departments
            );

            // 2️⃣ Create KnowHowDetail
            var knowHow = new KnowHowDetail
            {
                ContentId = content.ContentId,
                Code = model.Code,
                RiskLevel = model.RiskLevel,
                IsActive = true
            };
            _db.KnowHowDetails.Add(knowHow);

            // 3️⃣ Save content + knowhow first
            await _db.SaveChangesAsync();

            // 4️⃣ Create Items
            if (model.Items.Any())
            {
                await _itemService.AddItemsAsync(content.ContentId, model.Items);
            }

            return content.ContentId;
        }


        public async Task<KnowHowDetail?> GetKnowHowDetailAsync(int contentId)
        {
            return await _db.KnowHowDetails
                .Include(k => k.Content)
                    .ThenInclude(c => c.ContentDepartments)
                        .ThenInclude(cd => cd.Department)
                .Include(k => k.Content)
                    .ThenInclude(c => c.Items)
                .Include(k => k.Content)
                    .ThenInclude(c => c.CreatedByNavigation)
                .FirstOrDefaultAsync(k => k.ContentId == contentId && k.IsActive);
        }
    }
}

