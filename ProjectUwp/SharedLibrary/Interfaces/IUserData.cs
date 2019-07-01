using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface IUserData
    {
        string Name { get; set; }
        int Age { get; set; }
        string Job { get; set; }
        string School { get; set; }
        string Description { get; set; }
        Gender Gender { get; set; }
    }
}
