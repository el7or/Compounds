using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Common.Models
{
    public class UserIdentity
    {
        public UserIdentity(Guid? id)
        {
            Id = id;
        }

        public UserIdentity(Guid? id, Guid? companyId)
        {
            Id = id;
            CompanyId = companyId;
        }

        public Guid? Id { get; private set; }
        public Guid? CompanyId { get; private set; }
    }
}
