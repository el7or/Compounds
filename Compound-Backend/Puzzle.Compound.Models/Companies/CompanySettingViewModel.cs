using System.Collections.Generic;

namespace Puzzle.Compound.Models.Companies
{
    public class CompanySettingViewModel
    {
        public bool Emergency { get; set; }
        public Dictionary<string, string> Services { get; set; }
        public bool Visits { get; set; }
    }
}
