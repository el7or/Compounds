using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.SystemPageActions
{
    public class SystemPageActionInfoViewModel
    {
        public Guid SystemPageActionId { get; set; }
        public string ActionArabicName { get; set; }
        public string ActionEnglishName { get; set; }
        public string ActionUniqueName { get; set; }
        public bool? IsSelected { get; set; }
    }
}
