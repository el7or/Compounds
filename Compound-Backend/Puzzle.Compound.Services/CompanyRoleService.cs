using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.CompanyRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Services
{
    public interface ICompanyRoleService
    {
        OperationState AddRole(AddEditCompanyRoleViewModel role);
        OperationState EditRole(AddEditCompanyRoleViewModel role);
        OperationState DeleteRole(Guid roleId);
        CompanyRoleInfoViewModel GetRoleById(Guid roleId);
        PagedOutput<CompanyRoleInfoViewModel> GetRolesByName(CompanyRoleFilterByTextInputViewModel model);
        PagedOutput<CompanyRoleInfoViewModel> GetRolesByCompanyId(CompanyRoleFilterByTextInputViewModel model);
    }

    public class CompanyRoleService : BaseService, ICompanyRoleService
    {
        private readonly ICompanyRoleRepository companyRoleRepository;
        private readonly ICompanyUserRoleRepository companyUserRoleRepository;
        private readonly ICompanyRoleActionsRepository companyRoleActionsRepository;

        public CompanyRoleService(ICompanyRoleRepository companyRoleRepository, IUnitOfWork unitOfWork,
            IMapper mapper, ICompanyUserRoleRepository companyUserRoleRepository, ICompanyRoleActionsRepository companyRoleActionsRepository)
            : base(unitOfWork, mapper)
        {
            this.companyRoleRepository = companyRoleRepository;
            this.companyUserRoleRepository = companyUserRoleRepository;
            this.companyRoleActionsRepository = companyRoleActionsRepository;
        }

        public PagedOutput<CompanyRoleInfoViewModel> GetRolesByName(CompanyRoleFilterByTextInputViewModel model)
        {
            var roles = companyRoleRepository.GetMany(r => r.RoleArabicName.ToLower().Contains(model.Text.ToLower())
                                                        || r.RoleEnglishName.ToLower().Contains(model.Text.ToLower()));


            return GetPaged(model, roles);
        }

        public PagedOutput<CompanyRoleInfoViewModel> GetRolesByCompanyId(CompanyRoleFilterByTextInputViewModel model)
        {
            var roles = companyRoleRepository.GetMany(c => c.CompanyId == model.CompanyId);

            return GetPaged(model, roles);
        }

        public CompanyRoleInfoViewModel GetRoleById(Guid id)
        {
            var role = companyRoleRepository.Get(g => g.CompanyRoleId == id);
            return mapper.Map<CompanyRole, CompanyRoleInfoViewModel>(role);
        }

        public CompanyRole GetRoleByName(string arabicName, string englishName)
        {
            return companyRoleRepository.Get(c => c.RoleArabicName.ToLower().Contains(arabicName.ToLower()) || c.RoleEnglishName.ToLower().Contains(englishName.ToLower()));
        }

        public OperationState AddRole(AddEditCompanyRoleViewModel role)
        {
            var existingRole = GetRoleByName(role.RoleArabicName, role.RoleEnglishName);
            if (existingRole != null)
            {
                return OperationState.Exists;
            }

            var mappedRole = mapper.Map<AddEditCompanyRoleViewModel, CompanyRole>(role);

            mappedRole.CreationDate = DateTime.UtcNow;
            companyRoleRepository.Add(mappedRole);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                role.CompanyRoleId = mappedRole.CompanyRoleId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public OperationState EditRole(AddEditCompanyRoleViewModel updatedRole)
        {
            var role = companyRoleRepository.Get(g => g.CompanyRoleId == updatedRole.CompanyRoleId);

            var existingRole = GetRoleByName(updatedRole.RoleArabicName, updatedRole.RoleEnglishName);
            if (existingRole != null && existingRole.CompanyRoleId != updatedRole.CompanyRoleId)
            {
                return OperationState.Exists;
            }

            role.RoleArabicName = updatedRole.RoleArabicName;
            role.RoleEnglishName = updatedRole.RoleEnglishName;
            role.ModificationDate = DateTime.UtcNow;

            companyRoleRepository.Update(role);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteRole(Guid roleId)
        {
            var role = companyRoleRepository.Get(g => g.CompanyRoleId == roleId);
            if (role != null)
            {
                companyUserRoleRepository.Delete(u => u.CompanyRoleId == roleId);
                companyRoleActionsRepository.Delete(u => u.CompanyRoleId == roleId);
                companyRoleRepository.Delete(role);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        private PagedOutput<CompanyRoleInfoViewModel> GetPaged(CompanyRoleFilterByTextInputViewModel model, IEnumerable<CompanyRole> roles)
        {
            var output = new PagedOutput<CompanyRoleInfoViewModel>
            {
                TotalCount = roles.Count()
            };
            var result = companyRoleRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<CompanyRoleInfoViewModel>>(result.ToList());

            return output;
        }
    }
}
