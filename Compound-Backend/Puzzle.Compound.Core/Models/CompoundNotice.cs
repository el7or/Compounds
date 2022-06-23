using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundNotice
    {
        public Guid CompoundNoticeId { get; set; }
        public string NoticeContent { get; set; }
        public DateTime NoticeDate { get; set; }
        public Guid CompanyUserId { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual CompanyUser CompanyUser { get; set; }
        public virtual Compound Compound { get; set; }
    }
}
