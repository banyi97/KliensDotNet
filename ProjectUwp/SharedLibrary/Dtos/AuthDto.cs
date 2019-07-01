using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class AuthDto
    {
        public string AuthToken { get; set; }
        public bool Succeeded { get; set; }
        public string Error { get; set; }
    }
}
