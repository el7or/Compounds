using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Common.Models
{
    public class PagedInput
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public bool IsSortAscending { get; set; }
    }
}
