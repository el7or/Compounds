using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Ads
{
   public class AdOutputViewModel
    {
        public Guid CompoundAdId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsUrl { get; set; }
        public string AdUrl { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public int ShowsCount { get; set; }
        public int ClicksCount { get; set; }
        public int UniqueShowsCount { get; set; }
        public int UniqueClicksCount { get; set; }
        public bool IsActive { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
    public class AdMobileOutputViewModel
    {
        public Guid CompoundAdId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsUrl { get; set; }
        public string AdUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
}
