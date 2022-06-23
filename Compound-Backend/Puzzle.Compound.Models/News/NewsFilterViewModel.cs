using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.News
{
   public class NewsFilterViewModel: PagedInput
    {
        public Guid? CompoundId { get; set; }
        public Guid? CompanyId { get; set; }
        public DateTime? PublishDateFrom { get; set; }
        public DateTime? PublishDateTo { get; set; }
        public bool? IsActive { get; set; }
        public string SearchText { get; set; }
    }
}
