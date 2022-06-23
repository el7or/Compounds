using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class VisitorRequestRepository : RepositoryBase<VisitRequest>, IVisitorRequestRepository
    {
        public VisitorRequestRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IVisitorRequestRepository : IRepository<VisitRequest>
    {
    }
}
