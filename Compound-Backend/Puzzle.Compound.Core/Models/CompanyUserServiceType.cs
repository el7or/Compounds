using System;

namespace Puzzle.Compound.Core.Models
{
    public class CompanyUserServiceType
    {
        public Guid CompanyUserServiceId { get; set; }
        public Guid CompanyUserCompoundId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual CompanyUserCompound CompanyUserCompound { get; set; }
    }
}
