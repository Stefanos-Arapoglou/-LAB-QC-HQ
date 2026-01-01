using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class Legislation
{
    public int LegislationId { get; set; }

    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Link { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
}
