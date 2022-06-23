using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public partial class SystemPage
    {
        public SystemPage()
        {
            SubPages = new HashSet<SystemPage>();
            SystemPageActions = new HashSet<SystemPageAction>();
        }

        public Guid SystemPageId{ get; set; }
        public string PageArabicName { get; set; }
        public string PageEnglishName { get; set; }
        public int PageIndex { get; set; }
        public string PageURL { get; set; }
        public Guid ParentPageId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual SystemPage ParentPage { get; set; }

        public virtual ICollection<SystemPage> SubPages { get; set; }
        public virtual ICollection<SystemPageAction> SystemPageActions { get; set; }
    }
}
