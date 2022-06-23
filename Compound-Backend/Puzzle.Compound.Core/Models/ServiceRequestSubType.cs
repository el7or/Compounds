using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class ServiceRequestSubType
    {
        public Guid ServiceRequestSubTypeId { get; set; }
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceSubTypeId { get; set; }
        public decimal ServiceSubTypeCost { get; set; }
        public int ServiceSubTypeQuantity { get; set; }
        public int Order { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; }
        public virtual ServiceSubType ServiceSubType { get; set; }
    }
}
