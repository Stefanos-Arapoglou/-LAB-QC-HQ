using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class Content
{
    public int ContentId { get; set; }

    public string ContentType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int TimesViewed { get; set; }

    public virtual ICollection<ContentDepartment> ContentDepartments { get; set; } = new List<ContentDepartment>();

    public virtual ApplicationUser CreatedByNavigation { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual KnowHowDetail? KnowHowDetail { get; set; }

    public virtual ICollection<Legislation> Legislations { get; set; } = new List<Legislation>();
}
