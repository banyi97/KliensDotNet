using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface IPosition
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
