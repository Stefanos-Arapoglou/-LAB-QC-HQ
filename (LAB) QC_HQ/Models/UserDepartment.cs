using _LAB__QC_HQ.MetaData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

[ModelMetadataType(typeof(UserDepartmentMetadata))]
public partial class UserDepartment
{
    public string UserId { get; set; } = null!;

    public int DepartmentId { get; set; }

    public byte ClearanceLevel { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ApplicationUser User { get; set; }
}
