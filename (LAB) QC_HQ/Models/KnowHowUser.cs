using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class KnowHowUser
{
    public int ContentId { get; set; }

    public string UserId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime AssignedAt { get; set; }

    public virtual KnowHowDetail Content { get; set; } = null!;

    public virtual ApplicationUser User { get; set; }
}
