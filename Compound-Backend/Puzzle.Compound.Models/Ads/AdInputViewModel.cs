using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Ads
{
    public class AdInputViewModel
    {
        public Guid? CompoundAdId { get; set; }
        public Guid CompoundId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdUrl { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public bool IsActive { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
}
