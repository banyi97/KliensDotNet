using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime LatestUpdate { get; set; }
    }
}
