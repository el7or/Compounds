using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class GateRepository : RepositoryBase<Gate>, IGateRepository
    {
        public GateRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IGateRepository : IRepository<Gate>
    {
    }
}
