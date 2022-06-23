using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompanyUserRoleRepository : RepositoryBase<CompanyUserRole>, ICompanyUserRoleRepository
    {
        private readonly CompoundDbContext _dbContext;
        public CompanyUserRoleRepository(CompoundDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string> GetActionsInCompanyRoles(List<Guid> rolesIds)
        {
           return _dbContext.ActionsInCompanyRoles.Where(a => rolesIds.Contains(a.CompanyRoleId))
                .Include(a => a.SystemPageActions).Select(a => a.SystemPageActions.ActionUniqueName).ToList();
        }
    }

    public interface ICompanyUserRoleRepository : IRepository<CompanyUserRole>
    {
        List<string> GetActionsInCompanyRoles(List<Guid> rolesIds);
    }
}
