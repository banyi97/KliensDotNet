using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
    public interface ISearchParameters
    {
        int MinAge { get; set; }
        int MaxAge { get; set; }
        double MaxDist { get; set; }
        Gender SearchedGender { get; set; }
    }
}
