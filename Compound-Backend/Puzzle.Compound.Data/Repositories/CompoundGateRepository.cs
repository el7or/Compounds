using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundGateRepository : RepositoryBase<CompoundGate>, ICompoundGateRepository
    {
        public CompoundGateRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundGateRepository : IRepository<CompoundGate>
    {
    }
}
