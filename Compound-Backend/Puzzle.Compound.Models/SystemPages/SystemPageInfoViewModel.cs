using Puzzle.Compound.Models.SystemPageActions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.SystemPages
{
    public class SystemPageInfoViewModel
    {

        public Guid SystemPageId { get; set; }
        public string PageArabicName { get; set; }
        public string PageEnglishName { get; set; }
        public int PageIndex { get; set; }
        public string PageURL { get; set; }
        public Guid ParentPageId { get; set; }

        public IEnumerable<SystemPageActionInfoViewModel> SystemPageActions { get; set; }
    }
}
