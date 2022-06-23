using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class PrintCardRequestRepository : RepositoryBase<PrintCardRequest>, IPrintCardRequestRepository
    {
        public PrintCardRequestRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IPrintCardRequestRepository : IRepository<PrintCardRequest>
    {
    }
}
