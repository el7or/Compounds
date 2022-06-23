using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Authentication;
using Puzzle.Compound.Models.CompanyRoles;
using Puzzle.Compound.Models.CompanyUsers;
using Puzzle.Compound.Models.Compounds;
using Puzzle.Compound.Models.SystemPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompanyUserService
    {
        //CompanyUser GetUser(string username, string password);

        Task<CompanyUser> GetUserByIdAsync(Guid id);
        IEnumerable<CompanyUser> GetByCompanyId(Guid id);
        Task<OperationState> AddUser(CompanyUser companyUser, CompoundUserInfo[] compounds = null);
        Task<OperationState> EditUser(CompanyUser companyUser, CompoundUserInfo[] compounds = null);
        CompanyUser GetUser(string username, string password);
        Task<CompanyUser> GetByUsername(string username);
        Task<OperationState> DeleteUser(Guid compoundId);
        Task<PagedOutput<CompanyUserList>> FilterCompanyUsers(CompanyUserFilter filter);
        Task<OperationState> activateUser(Guid userId, bool active);

        UserAuthenticationResponseModel GetCompanyUserAccessInformation(Guid companyUserId);
        List<CompoundResponseModel> GetUserCompoundsIds(Guid userId);
    }

    public class CompanyUserService : BaseService, ICompanyUserService
    {
        private readonly ICompanyUserRepository companyUserRepository;
        private readonly ICompanyUserRoleService companyUserRoleService;
        private readonly ICompanyRoleService companyRoleService;
        private readonly ICompoundService compoundService;
        private readonly IHubContext<CounterHub> _hub;

        public CompanyUserService(ICompanyUserRepository companyUserRepository,
                ICompanyUserRoleService companyUserRoleService,
                ICompanyRoleService companyRoleService,
                ICompoundService compoundService,
                 IUnitOfWork unitOfWork,
                IMapper mapper,
                IHubContext<CounterHub> hub)
                : base(unitOfWork, mapper)
        {
            this.companyUserRepository = companyUserRepository;
            this.companyUserRoleService = companyUserRoleService;
            this.companyRoleService = companyRoleService;
            this.compoundService = compoundService;
            _hub = hub;
        }
        public CompanyUser GetUser(string username, string password)
        {
            return companyUserRepository.Table
                .Where(u => u.Username == username && u.Password == password)
                .Include(c => c.CompanyUserCompounds)
                .Include(ur => ur.CompanyUserRoles)
                    .ThenInclude(r => r.CompanyRole)
                        .ThenInclude(a => a.ActionsInCompanyRoles)
                            .ThenInclude(pa => pa.SystemPageActions)
                .FirstOrDefault();
        }

        public async Task<CompanyUser> GetUserByIdAsync(Guid id)
        {
            return await companyUserRepository.GetByIdAsync(id);
            //return await companyUserRepository.Table.Where(u => u.CompanyUserId == id)
            //    .Include(c => c.CompanyUserCompounds)
            //        .ThenInclude(s => s.CompanyUserServices)
            //    .Include(c => c.CompanyUserCompounds)
            //        .ThenInclude(i => i.CompanyUserIssues)
            //    .FirstOrDefaultAsync();
        }

        public IEnumerable<CompanyUser> GetByCompanyId(Guid id)
        {
            return companyUserRepository.GetMany(u => u.CompanyId == id && u.IsDeleted == false);
        }

        public async Task<OperationState> AddUser(CompanyUser companyUser, CompoundUserInfo[] compounds = null)
        {
            var existsUser = await GetByUsername(companyUser.Username);
            if (existsUser != null)
            {
                return OperationState.Exists;
            }
            companyUser.IsActive = true;
            companyUser.IsDeleted = false;
            companyUser.IsVerified = false;
            companyUser.CreationDate = DateTime.UtcNow;

            if (compounds != null && compounds.Any())
            {
                foreach (var userCompound in compounds)
                {
                    companyUser.CompanyUserCompounds.Add(new CompanyUserCompound
                    {
                        AssignedDate = DateTime.Now,
                        CompoundId = userCompound.CompoundId,
                        IsActive = true,
                        IsDeleted = false,
                        CompanyUserServices = userCompound.Services.Select(z => new CompanyUserServiceType
                        {
                            AssignedDate = DateTime.Now,
                            ServiceTypeId = z,
                            IsActive = true,
                            IsDeleted = false
                        }).ToArray(),
                        CompanyUserIssues = userCompound.Issues.Select(z => new CompanyUserIssueType
                        {
                            AssignedDate = DateTime.Now,
                            IssueTypeId = z,
                            IsActive = true,
                            IsDeleted = false
                        }).ToArray()
                    });
                }
            }
            companyUserRepository.Add(companyUser);
            int result = await unitOfWork.CommitAsync();
            if (result > 0)
                return OperationState.Created;
            return OperationState.None;
        }

        public async Task<OperationState> EditUser(CompanyUser companyUser, CompoundUserInfo[] compounds = null)
        {
            var existsUser = await GetUserByIdAsync(companyUser.CompanyUserId);

            if (existsUser == null)
            {
                return OperationState.NotExists;
            }

            existsUser.Username = companyUser.Username;
            existsUser.Password = companyUser.Password;
            existsUser.NameAr = companyUser.NameAr;
            existsUser.NameEn = companyUser.NameEn;
            existsUser.Phone = companyUser.Phone;
            existsUser.Email = companyUser.Email;
            if (companyUser.Image != null)
                existsUser.Image = companyUser.Image;
            foreach (var userComp in existsUser.CompanyUserCompounds)
            {
                userComp.IsActive = false;
                userComp.IsDeleted = true;
                foreach (var serv in userComp.CompanyUserServices)
                {
                    serv.IsActive = false;
                    serv.IsDeleted = true;
                }
                foreach (var issue in userComp.CompanyUserIssues)
                {
                    issue.IsActive = false;
                    issue.IsDeleted = true;
                }
            }
            //var newCompounds = compounds.Where(z => existsUser.CompanyUserCompounds.All(x => x.CompoundId != z.CompoundId)).ToList();
            foreach (var compound in compounds)
                existsUser.CompanyUserCompounds.Add(new CompanyUserCompound
                {
                    CompoundId = compound.CompoundId,
                    AssignedDate = DateTime.Now,
                    CompanyUserServices = compound.Services.Select(x => new CompanyUserServiceType
                    {
                        AssignedDate = DateTime.Now,
                        ServiceTypeId = x
                    }).ToArray(),
                    CompanyUserIssues = compound.Issues.Select(x => new CompanyUserIssueType
                    {
                        AssignedDate = DateTime.Now,
                        IssueTypeId = x
                    }).ToArray()
                });
            //var updatedCompounds = compounds.Where(z => existsUser.CompanyUserCompounds.Any(x => x.CompoundId == z.CompoundId)).ToList();
            companyUserRepository.Update(existsUser);
            int result = await unitOfWork.CommitAsync();
            await _hub.Clients.All.SendAsync("ForceLogoutUser", companyUser.CompanyUserId.ToString());

            return result > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> DeleteUser(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return OperationState.NotExists;
            }

            user.IsDeleted = true;
            companyUserRepository.Update(user);
            var result = await unitOfWork.CommitAsync();
            await _hub.Clients.All.SendAsync("ForceLogoutUser", userId.ToString());

            return result > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<CompanyUser> GetByUsername(string username)
        {
            return await companyUserRepository.Table.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<OperationState> activateUser(Guid userId, bool active)
        {
            var existsUser = await GetUserByIdAsync(userId);
            if (existsUser == null)
            {
                return OperationState.NotExists;
            }
            existsUser.IsActive = active;
            companyUserRepository.Update(existsUser);
            int result = await unitOfWork.CommitAsync();
            if (!active)
                await _hub.Clients.All.SendAsync("ForceLogoutUser", userId.ToString());
            return result > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<CompanyUserList>> FilterCompanyUsers(CompanyUserFilter filter)
        {
            var users = companyUserRepository.Table
                .Include(u => u.CompanyUserRoles)
                    .ThenInclude(r => r.CompanyRole)
                        .ThenInclude(r => r.ActionsInCompanyRoles)
                .Where(y => y.IsDeleted != true && y.CompanyId == filter.CompanyId);

            if (!string.IsNullOrEmpty(filter.SearchText))
                users = users.Where(z => z.NameAr.Contains(filter.SearchText) ||
                z.NameEn.Contains(filter.SearchText) || z.Email.Contains(filter.SearchText)
                || z.Username.Contains(filter.SearchText) || z.Phone.Contains(filter.SearchText));

            // apply sorting
            var columns = new Dictionary<string, Expression<Func<CompanyUser, object>>>()
            {
                ["nameAr"] = v => v.NameAr,
                ["nameEn"] = v => v.NameEn,
                ["isActive"] = v => v.IsActive,

            };
            users = users.ApplySorting(filter, columns);

            var filteredUsers = users
                    .Select(z => new CompanyUserList()
                    {
                        CompanyUserId = z.CompanyUserId,
                        NameAr = z.NameAr,
                        NameEn = z.NameEn,
                        IsActive = z.IsActive,
                        Username = z.Username,
                        Email = z.Email,
                        Phone = z.Phone,
                        IsSelected = filter.CompanyRoleId.HasValue ? z.CompanyUserRoles.Any(r => r.CompanyRoleId == filter.CompanyRoleId) : false,
                        CurrentRole = z.CompanyUserRoles.Any() ? mapper.Map<CompanyCurrentRoleViewModel>(z.CompanyUserRoles.First().CompanyRole) : null
                    });

            var pageUsers = await filteredUsers.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<CompanyUserList>
            {
                TotalCount = filteredUsers.Count(),
                Result = pageUsers
            };
        }

        public UserAuthenticationResponseModel GetCompanyUserAccessInformation(Guid companyUserId)
        {
            var accessInfo = new UserAuthenticationResponseModel();

            var userRoles = companyUserRoleService.GetUserRolesByUserId(companyUserId);

            accessInfo.RolesIds = userRoles.Select(r => r.CompanyRoleId).ToList();
            accessInfo.Compounds = GetUserCompoundsIds(companyUserId);
            accessInfo.UserActions = companyUserRoleService.GetActionsInCompanyRoles(accessInfo.RolesIds);

            return accessInfo;
        }

        public List<CompoundResponseModel> GetUserCompoundsIds(Guid userId)
        {
            return companyUserRepository.GetUserCompoundsIds(userId);
        }
    }
}
