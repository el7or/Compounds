using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.SystemPageActions
{
    public class SystemPageActionFilterByTextInputViewModel : PagedInput
    {
        public Guid SystemPageId { get; set; }
        public string Text { get; set; }
    }
}
