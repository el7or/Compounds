using Puzzle.Compound.Core.Models;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Infrastructure
{
    public class UnitOfWork : Disposable, IUnitOfWork
    {
        private CompoundDbContext _dbContext;
        public UnitOfWork(CompoundDbContext dbCOntext)
        {
            _dbContext = dbCOntext;
        }
        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        protected override void DisposeCore()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
    }
}
