using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class ServiceSubType
    {
        public ServiceSubType()
        {
            ServiceRequestSubTypes = new HashSet<ServiceRequestSubType>();
        }
        public Guid ServiceSubTypeId { get; set; }
        public Guid CompoundServiceId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }
        public decimal Cost { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public virtual CompoundService CompoundService { get; set; }
        public virtual ICollection<ServiceRequestSubType> ServiceRequestSubTypes { get; set; }
    }
}
