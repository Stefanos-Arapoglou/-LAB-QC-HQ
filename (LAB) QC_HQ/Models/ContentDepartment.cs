using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class ContentDepartment
{
    public int ContentId { get; set; }

    public int DepartmentId { get; set; }

    public byte ClearanceLevelRequired { get; set; }

    public virtual Content Content { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;
}
