using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services
{
    public class CompoundServiceOutput
    {
        public Guid ServiceTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public List<ServiceSubTypeOutputViewModel> ServiceSubTypes { get; set; }
    }
}
