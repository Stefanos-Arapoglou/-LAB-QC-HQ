using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace _LAB__QC_HQ.MetaData
{
    public class UserDepartmentEditMetadata
    {
        [BindNever]
        public string DepartmentName { get; set; }
    }
}
