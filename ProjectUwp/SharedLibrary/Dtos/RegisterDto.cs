using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
