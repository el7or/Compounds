using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services
{
    public class ServiceRequestList
    {
        public Guid ServiceRequestId { get; set; }
        public int RequestNumber { get; set; }
        public string RequestedBy { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ServiceTypeArabic { get; set; }
        public string ServiceTypeEnglish { get; set; }
        public short Status { get; set; }
        public DateTime PostTime { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public short Rate { get; set; }
        public string Date { get; set; }
        public string Icon { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string UnitName { get; set; }
        public decimal ServiceSubTypesTotalCost { get; set; }
        public List<ServiceRequestSubTypeOutPutViewModel> ServiceSubTypes { get; set; }
    }
}
