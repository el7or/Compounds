using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public class IssueRequest
    {
        public IssueRequest()
        {
            IssueAttachments = new HashSet<IssueAttachment>();
        }
        public Guid IssueRequestId { get; set; }
        public Guid IssueTypeId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public int RequestNumber { get; set; }
        public DateTime PostTime { get; set; }

        // 0 => pending , 1 => done , 2 => cancelled
        public short Status { get; set; }
        public DateTime UpdateStatusTime { get; set; }
        public Guid UpdateStatusBy { get; set; }
        public Guid CompoundId { get; set; }
        public string Note { get; set; }
        public short Rate { get; set; }
        public short PresenterRate { get; set; }
        public string Comment { get; set; }
        public string OwnerComment { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        // 0 => owner , 1 => admin
        public short CancelType { get; set; }
        public string Record { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual IssueType IssueType { get; set; }
        public virtual Compound Compound { get; set; }
        public virtual ICollection<IssueAttachment> IssueAttachments { get; set; }
    }
}
