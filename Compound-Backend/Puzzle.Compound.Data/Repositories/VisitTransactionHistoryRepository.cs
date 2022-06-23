using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{

    public interface IVisitTransactionHistoryRepository : IRepository<VisitTransactionHistory>
    {

    }
    public class VisitTransactionHistoryRepository : RepositoryBase<VisitTransactionHistory>, IVisitTransactionHistoryRepository
    {
        public VisitTransactionHistoryRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }
}
