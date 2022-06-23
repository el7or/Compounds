using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class Transaction
    {
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string RecordId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PostDateTime { get; set; }
        public string TableName { get; set; }
        public string CurrentJsonData { get; set; }
        public string PreviousJsonData { get; set; }
        public string ControllerPath { get; set; }
        public string ActionPath { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}
