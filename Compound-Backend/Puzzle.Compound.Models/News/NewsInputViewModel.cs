﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.News
{
   public class NewsInputViewModel
    {
        public Guid? CompoundNewsId { get; set; }
        public Guid CompoundId { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishSummary { get; set; }
        public string ArabicSummary { get; set; }
        public string EnglishDetails { get; set; }
        public string ArabicDetails { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ForegroundTillDate { get; set; }
        public bool IsActive { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
}
