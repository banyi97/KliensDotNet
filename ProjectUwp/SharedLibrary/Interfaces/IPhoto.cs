using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface IPhoto
    {
        bool Main { get; set; }
        string FileName { get; set; }
        string Extenstion { get; set; }
        string Url { get; set; }
    }
}
