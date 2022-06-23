using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompanyUserRepository : RepositoryBase<CompanyUser>, ICompanyUserRepository
    {
        private readonly CompoundDbContext _dbContext;
        public CompanyUserRepository(CompoundDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CompoundResponseModel> GetUserCompoundsIds(Guid userId)
        {
            return _dbContext.CompanyUserCompounds.Where(u => u.CompanyUserId == userId)
                .Select(c => new CompoundResponseModel
                {
                    CompoundId = c.CompoundId,
                    ServiceTypesIds = c.CompanyUserServices.Select(s => s.ServiceTypeId).ToList(),
                    IssueTypesIds = c.CompanyUserIssues.Select(s => s.IssueTypeId).ToList()
                }).ToList();
        }
    }

    public interface ICompanyUserRepository : IRepository<CompanyUser>
    {
        List<CompoundResponseModel> GetUserCompoundsIds(Guid userId);
    }
}
