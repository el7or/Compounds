using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{

    public class VisitRequestAttachementRepository : RepositoryBase<VisitRequestAttachment>, IVisitAttachementRepository
    {
        public VisitRequestAttachementRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IVisitAttachementRepository : IRepository<VisitRequestAttachment>
    {
    }
}
