using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundAdHistoryRepository : RepositoryBase<CompoundAdHistory>, ICompoundAdHistoryRepository
    {
        public CompoundAdHistoryRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundAdHistoryRepository : IRepository<CompoundAdHistory>
    {
    }
}
