using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface IMessage
    {
        string SenderId { get; set; }
        string Data { get; set; }
    }
}
