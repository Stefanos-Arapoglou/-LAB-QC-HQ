using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Interfaces;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using _LAB__QC_HQ.Models.Enums;

namespace _LAB__QC_HQ.Services
{
    public class KnowHowService : IKnowHowService
    {
        private readonly ApplicationDbContext _db;
        private readonly IContentService _contentService;

        public KnowHowService(
            ApplicationDbContext db,
            IContentService contentService)
        {
            _db = db;
            _contentService = contentService;
        }

        public int CreateKnowHow(CreateKnowHowViewModel model, string userId)
        {
            var content = _contentService.CreateContent(
                model.Title,
                ContentType.KnowHow,
                userId,
                model.Departments);

            var knowHow = new KnowHowDetail
            {
                ContentId = content.ContentId,
                Code = model.Code,
                RiskLevel = model.RiskLevel,
                IsActive = true
            };

            _db.KnowHowDetails.Add(knowHow);
            _db.SaveChanges();

            return content.ContentId;
        }
    }
}
