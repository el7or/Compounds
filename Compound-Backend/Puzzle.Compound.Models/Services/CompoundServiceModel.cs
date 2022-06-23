using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services
{
    public class CompoundServiceModel
    {
        public Guid ServiceTypeId { get; set; }
        public string ServiceNameArabic { get; set; }
        public string ServiceNameEnglish { get; set; }
        public bool Selected { get; set; }
        public int ServiceOrder { get; set; }
        public int AssignOrder { get; set; }
        public bool IsFixed { get; set; }
        public List<ServiceSubTypeOutputViewModel> ServiceSubTypes { get; set; }
    }
}
