using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ContentDepartment> ContentDepartments { get; set; } = new List<ContentDepartment>();

    public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
}
