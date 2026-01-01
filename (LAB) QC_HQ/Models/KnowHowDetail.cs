using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class KnowHowDetail
{
    public int ContentId { get; set; }

    public string? Code { get; set; }

    public byte RiskLevel { get; set; }

    public DateTime? LastReviewedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Content Content { get; set; } = null!;

    public virtual ICollection<KnowHowUser> KnowHowUsers { get; set; } = new List<KnowHowUser>();
}
