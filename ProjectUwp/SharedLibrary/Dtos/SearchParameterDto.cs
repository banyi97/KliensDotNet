using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Dtos
{
    public class SearchParameterDto : ISearchParameters
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double MaxDist { get; set; }
        public Gender SearchedGender { get; set; }
    }
}
