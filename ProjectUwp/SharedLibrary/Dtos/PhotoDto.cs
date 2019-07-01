using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class PhotoDto : IPhoto
    {
        public string Id { get; set; }

        public bool Main { get; set; }
        public string FileName { get; set; }
        public string Extenstion { get; set; }
        public string Url { get; set; }
    }
}
