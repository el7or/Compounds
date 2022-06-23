using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Common.Models
{
    public class PagedOutput<T>
    {
        public int TotalCount { get; set; }
        public List<T> Result { get; set; }
    }
}
