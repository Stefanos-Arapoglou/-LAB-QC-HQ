using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.DTO;
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

        public async Task UpdateKnowHowAsync(int contentId, EditKnowHowViewModel model)
        {
            await _contentService.UpdateContentAsync(contentId, model.Title, model.Departments);

            var knowHow = await _db.KnowHowDetails.FirstOrDefaultAsync(k => k.ContentId == contentId);
            if (knowHow == null) throw new KeyNotFoundException();

            knowHow.Code = model.Code;
            knowHow.RiskLevel = model.RiskLevel;
            await _db.SaveChangesAsync();

            if (model.Items.Any())
            {
                await _itemService.UpdateItemsAsync(contentId, model.Items);
            }
        }

        public async Task<EditKnowHowViewModel?> GetEditViewModelAsync(int contentId)
        {
            var knowHowDetail = await _db.KnowHowDetails
                .Include(k => k.Content)
                    .ThenInclude(c => c.ContentDepartments)
                .Include(k => k.Content)
                    .ThenInclude(c => c.Items)
                .FirstOrDefaultAsync(k => k.ContentId == contentId && k.IsActive);

            if (knowHowDetail == null)
                return null;

            var model = new EditKnowHowViewModel
            {
                ContentId = knowHowDetail.ContentId,
                Title = knowHowDetail.Content.Title,
                Code = knowHowDetail.Code,
                RiskLevel = knowHowDetail.RiskLevel,
                Departments = knowHowDetail.Content.ContentDepartments
                    .Select(cd => new DepartmentClearanceInput
                    {
                        DepartmentId = cd.DepartmentId,
                        ClearanceLevelRequired = cd.ClearanceLevelRequired
                    }).ToList(),
                Items = knowHowDetail.Content.Items
    .Select(i => new EditItemInput
    {
        ItemId = i.ItemId,
        ItemType = i.ItemType,
        ItemValue = i.ItemValue,
        ItemTitle = i.ItemTitle
    }).ToList()
            };

            return model;
        }
    }
}

