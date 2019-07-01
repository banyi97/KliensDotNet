using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class UserDataSettingsDto 
    {
        public string Job { get; set; }
        public string School { get; set; }
        public string Description { get; set; }
    }
}
