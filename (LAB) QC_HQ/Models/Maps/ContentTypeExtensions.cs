using _LAB__QC_HQ.Models.Enums;
namespace _LAB__QC_HQ.Models.Maps
{
    public static class ContentTypeExtensions
    {
        public static string ToDbString(this ContentType type)
        {
            return type switch
            {
                ContentType.KnowHow => "Know-How",
                ContentType.Educational => "Educational",
                ContentType.Announcement => "Announcement",
                ContentType.File => "File",
                _ => throw new InvalidOperationException($"Unknown content type {type}")
            };
        }
    }
}
