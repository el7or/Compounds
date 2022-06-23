using System;

namespace Puzzle.Compound.Core.Models
{
    public class RegistrationForUser
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string RegisterId { get; set; }
        public Guid UserId { get; set; }
        public string RegisterType { get; set; }
        public virtual OwnerRegistration User { get; set; }
    }
}