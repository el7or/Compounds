using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Infrastructure
{
    public interface IUnitOfWork<TContext> where TContext : CompoundDbContext
    {
        TContext _dbContext { get; }
        int Commit();
        Task<int> CommitAsync();
    }
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        Task<int> CommitAsync();
    }
}
