using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace _LAB__QC_HQ.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public int ContentId { get; set; }

    public string ItemType { get; set; } = null!;

    [Column("item_title")]
    public string ItemTitle { get; set; } = null!;

    public string ItemValue { get; set; } = null!;

    public virtual Content Content { get; set; } = null!;
}
