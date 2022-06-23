using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
   public class CompoundAdRepository : RepositoryBase<CompoundAd>, ICompoundAdRepository
    {
        public CompoundAdRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundAdRepository : IRepository<CompoundAd>
    {
    }
}
