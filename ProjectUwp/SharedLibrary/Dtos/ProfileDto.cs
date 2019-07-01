using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class ProfileDto : IUserData
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Job { get; set; }
        public string School { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }

        public IList<PhotoDto> Photos = new List<PhotoDto>();
    }
}
