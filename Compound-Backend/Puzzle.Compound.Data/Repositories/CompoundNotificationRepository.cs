using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
   public class CompoundNotificationRepository : RepositoryBase<CompoundNotification>, ICompoundNotificationRepository
    {
        public CompoundNotificationRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }
    public interface ICompoundNotificationRepository : IRepository<CompoundNotification>
    {
    }
}
