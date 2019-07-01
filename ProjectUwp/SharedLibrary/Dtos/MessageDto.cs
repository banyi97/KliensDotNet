using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class MessageDto : IMessage
    {
        public string SenderId { get; set; }
        public string Data { get; set; }

        public DateTime Created { get; set; }
    }
}
