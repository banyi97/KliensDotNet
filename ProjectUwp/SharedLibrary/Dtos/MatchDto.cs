using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class MatchDto
    {
        public string Id { get; set; }

        public string LastMessage { get; set; }
        public string LastSenderId { get; set; }

        public string PicUrl { get; set; }
        public string Name { get; set; }
    }
}
