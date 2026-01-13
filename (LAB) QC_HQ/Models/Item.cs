using _LAB__QC_HQ.MetaData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace _LAB__QC_HQ.Models;

[ModelMetadataType(typeof(ItemMetadata))]
public partial class Item
{
    public int ItemId { get; set; }

    public int ContentId { get; set; }

    public string ItemType { get; set; } = null!;

    [Column("item_title")]
    public string ItemTitle { get; set; } = null!;

    public string ItemValue { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public virtual Content Content { get; set; } = null!;
}
