using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.CompanyUserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompanyUserRoleService
    {
        OperationState AddUserRole(AddEditCompanyUserRoleViewModel userRole);
        OperationState EditUserRole(AddEditCompanyUserRoleViewModel userRole);
        OperationState DeleteUserRole(Guid userRoleId);
        CompanyUserRoleInfoViewModel GetUserRoleInfoById(Guid userRoleId);
        CompanyUserRole GetUserRoleById(Guid userRoleId);
        PagedOutput<CompanyUserRoleInfoViewModel> GetUserRoles(CompanyUserRoleFilterByTextInputViewModel model);
        PagedOutput<CompanyUserRoleInfoViewModel> GetUserRolesByCompanyRoleId(CompanyUserRoleFilterByTextInputViewModel model);
        Task<OperationState> UpdateUsersRolesAsync(UpdateUserRoles model);
        List<string> GetActionsInCompanyRoles(List<Guid> rolesIds);
        IEnumerable<CompanyUserRoleInfoViewModel> GetUserRolesByUserId(Guid companyUserId);
    }

    public class CompanyUserRoleService : BaseService, ICompanyUserRoleService
    {
        private readonly ICompanyUserRoleRepository companyUserRoleRepository;
        private readonly IHubContext<CounterHub> _hub;

        public CompanyUserRoleService(ICompanyUserRoleRepository companyUserRoleRepository, IUnitOfWork unitOfWork,
            IMapper mapper, IHubContext<CounterHub> hub)
            : base(unitOfWork, mapper)
        {
            this.companyUserRoleRepository = companyUserRoleRepository;
            _hub = hub;
        }

        public OperationState AddUserRole(AddEditCompanyUserRoleViewModel userRole)
        {
            var existingRole = companyUserRoleRepository.Get(r => r.CompanyUserId == userRole.CompanyUserId && r.CompanyRoleId == userRole.CompanyRoleId);
            if(existingRole != null)
            {
                return OperationState.Exists;
            }
            userRole.AssignedDate = DateTime.UtcNow;

            var mappedUserRole = mapper.Map<AddEditCompanyUserRoleViewModel, CompanyUserRole>(userRole);

            mappedUserRole.CreationDate = DateTime.UtcNow;
            companyUserRoleRepository.Add(mappedUserRole);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                userRole.CompanyUserRoleId = mappedUserRole.CompanyUserRoleId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public PagedOutput<CompanyUserRoleInfoViewModel> GetUserRoles(CompanyUserRoleFilterByTextInputViewModel model)
        {
            var roles = companyUserRoleRepository.GetMany(r => r.CompanyUserId == model.CompanyUserId
                                                        || r.CompanyRoleId == model.CompanyRoleId);

            return GetPaged(model, roles);
        }

        public IEnumerable<CompanyUserRoleInfoViewModel> GetUserRolesByUserId(Guid companyUserId)
        {
            var roles = companyUserRoleRepository.GetMany(r => r.CompanyUserId == companyUserId);

            return mapper.Map<List<CompanyUserRoleInfoViewModel>>(roles);
        }

        public CompanyUserRole GetUserRoleById(Guid id)
        {
            return companyUserRoleRepository.Get(g => g.CompanyUserRoleId == id);
        }

        public CompanyUserRoleInfoViewModel GetUserRoleInfoById(Guid id)
        {
            var role = companyUserRoleRepository.Get(g => g.CompanyUserRoleId == id);
            return mapper.Map<CompanyUserRole, CompanyUserRoleInfoViewModel>(role);
        }

        public OperationState EditUserRole(AddEditCompanyUserRoleViewModel updatedUserRole)
        {
            var existingUserRole = companyUserRoleRepository.Get(r => r.CompanyUserId == updatedUserRole.CompanyUserId && r.CompanyRoleId == updatedUserRole.CompanyRoleId);

            if (existingUserRole != null && existingUserRole.CompanyRoleId == updatedUserRole.CompanyRoleId)
            {
                return OperationState.Exists;
            }

            existingUserRole.ModificationDate = DateTime.UtcNow;

            companyUserRoleRepository.Update(existingUserRole);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteUserRole(Guid userRoleId)
        {
            var role = companyUserRoleRepository.Get(g => g.CompanyRoleId == userRoleId);
            if (role != null)
            {
                companyUserRoleRepository.Delete(role);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public PagedOutput<CompanyUserRoleInfoViewModel> GetUserRolesByCompanyRoleId(CompanyUserRoleFilterByTextInputViewModel model)
        {
            var roles = companyUserRoleRepository.GetMany(c => c.CompanyRoleId == model.CompanyRoleId);

            return GetPaged(model, roles);
        }

        private PagedOutput<CompanyUserRoleInfoViewModel> GetPaged(CompanyUserRoleFilterByTextInputViewModel model, IEnumerable<CompanyUserRole> roles)
        {
            var output = new PagedOutput<CompanyUserRoleInfoViewModel>
            {
                TotalCount = roles.Count()
            };
            var result = companyUserRoleRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<CompanyUserRoleInfoViewModel>>(result.ToList());

            return output;
        }

        public async Task<OperationState> UpdateUsersRolesAsync(UpdateUserRoles model)
        {
            var oldUserRoles = companyUserRoleRepository.GetMany(r => r.CompanyRoleId == model.UsersRoles.First().CompanyRoleId || model.UsersRoles.Select(u => u.CompanyUserId).Contains(r.CompanyUserId));
            foreach (var item in oldUserRoles)
            {
                companyUserRoleRepository.Delete(item);
            }
            foreach (var item in model.UsersRoles)
            {
                companyUserRoleRepository.Add(new CompanyUserRole
                {
                    CompanyUserId = item.CompanyUserId,
                    CompanyRoleId = item.CompanyRoleId,
                    CreationDate = DateTime.UtcNow
                });
                await _hub.Clients.All.SendAsync("ForceLogoutUser", item.CompanyUserId.ToString());
            }
            var result = unitOfWork.Commit();
            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public List<string> GetActionsInCompanyRoles(List<Guid> rolesIds)
        {
           return companyUserRoleRepository.GetActionsInCompanyRoles(rolesIds);
        }
    }
}
