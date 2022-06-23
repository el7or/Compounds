using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public class ServiceType
    {
        public ServiceType()
        {
            CompoundServices = new HashSet<CompoundService>();
            ServiceRequests = new HashSet<ServiceRequest>();
            CompanyUserServices = new HashSet<CompanyUserServiceType>();
        }
        public Guid ServiceTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public bool IsFixed { get; set; }
        public virtual ICollection<CompoundService> CompoundServices { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
        public virtual ICollection<CompanyUserServiceType> CompanyUserServices { get; set; }
    }
}
