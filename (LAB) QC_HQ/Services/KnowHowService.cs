/* NOTES 
 
Supplement service for managing KnowHow content, including creation, retrieval, and updating of KnowHow specifics
Implements _contentService and _itemService for handling shared content and item logic.

SUMMARY:
    1) HANDLES GETTING KnowHowDetail by ContentId
    2) HANDLES GETTING EditKnowHowViewModel by ContentId
    3) HANDLES CREATING KnowHow content (uses _contentService and _itemService)
    4) HANDLES UPDATING KnowHow content (uses _contentService and _itemService)
 
 */

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



        // ~~~~~~~~~~~~~~~ GET METHODS ~~~~~~~~~~~~~~ //


        // Get KnowHowDetail by ContentId
        public async Task<KnowHowDetail?> GetKnowHowDetailAsync(int contentId)
        {
            var knowHow = await _db.KnowHowDetails
                .Include(k => k.Content)
                    .ThenInclude(c => c.ContentDepartments)
                        .ThenInclude(cd => cd.Department)
                .Include(k => k.Content)
                    .ThenInclude(c => c.Items)
                .Include(k => k.Content)
                    .ThenInclude(c => c.CreatedByNavigation)
                .FirstOrDefaultAsync(k => k.ContentId == contentId && k.IsActive);

            if (knowHow?.Content?.Items != null)
            {
                // Order items by DisplayOrder
                knowHow.Content.Items = knowHow.Content.Items
                    .OrderBy(i => i.DisplayOrder)
                    .ToList();
            }

            return knowHow;
        }



        // Get EditKnowHowViewModel by ContentId
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
                    .OrderBy(i => i.DisplayOrder)  // <-- ADD THIS LINE!
                    .Select(i => new EditItemInput
                    {
                        ItemId = i.ItemId,
                        ItemType = i.ItemType,
                        ItemValue = i.ItemValue,
                        ItemTitle = i.ItemTitle,
                        DisplayOrder = i.DisplayOrder
                    }).ToList()
            };

            return model;
        }





        // ~~~~~~~~~~~~~~~ CREATE / UPDATE METHODS ~~~~~~~~~~~~~~ //


        // Create KnowHow - Uses _contentService to create Content first
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



        // Update KnowHow - Uses _contentService to update Content first
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

        
    }
}

