using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.CompanyRoleActions;
using Puzzle.Compound.Models.SystemPageActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Services
{
    public interface ICompanyRoleActionsService
    {
        OperationState AddRoleAction(AddEditCompanyRoleActionViewModel roleAction);
        OperationState EditRoleAction(AddEditCompanyRoleActionViewModel roleAction);
        OperationState DeleteRoleAction(Guid actionsInCompanyRolesId);
        CompanyRoleActionInfoViewModel GetRoleActionById(Guid actionsInCompanyRolesId);
        PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActions(CompanyRoleActionFilterByTextInputViewModel model);
        PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActionsByCompanyUserRoleId(CompanyRoleActionFilterByTextInputViewModel model);
        PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActionsBySystemPageActionId(CompanyRoleActionFilterByTextInputViewModel model);
        OperationState UpdatePagesActionsInRoles(UpdatePagesActionsInRoles model);
    }

    public class CompanyRoleActionsService : BaseService, ICompanyRoleActionsService
    {
        private readonly ICompanyRoleActionsRepository companyRoleActionsRepository;

        public CompanyRoleActionsService(ICompanyRoleActionsRepository companyRoleActionsRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.companyRoleActionsRepository = companyRoleActionsRepository;
        }

        public OperationState AddRoleAction(AddEditCompanyRoleActionViewModel roleAction)
        {
            var existingRoleAction = companyRoleActionsRepository.Get(r => r.CompanyRoleId == roleAction.CompanyRoleId 
                                                                && r.SystemPageActionId == roleAction.SystemPageActionId);
            if (existingRoleAction != null)
            {
                return OperationState.Exists;
            }

            var mappedRoleAction = mapper.Map<AddEditCompanyRoleActionViewModel, ActionsInCompanyRoles>(roleAction);

            mappedRoleAction.CreationDate = DateTime.UtcNow;
            companyRoleActionsRepository.Add(mappedRoleAction);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                roleAction.ActionsInCompanyRolesId = mappedRoleAction.ActionsInCompanyRolesId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActions(CompanyRoleActionFilterByTextInputViewModel model)
        {
            var roleActions = companyRoleActionsRepository.GetMany(r => r.CompanyRoleId == model.CompanyRoleId
                                                        || r.SystemPageActionId == model.SystemPageActionId);


            return GetPaged(model, roleActions);
        }

        public CompanyRoleActionInfoViewModel GetRoleActionById(Guid actionsInCompanyRolesId)
        {
            var roleAction = companyRoleActionsRepository.Get(g => g.ActionsInCompanyRolesId == actionsInCompanyRolesId);
            return mapper.Map<ActionsInCompanyRoles, CompanyRoleActionInfoViewModel>(roleAction);
        }

        public OperationState EditRoleAction(AddEditCompanyRoleActionViewModel roleAction)
        {
            var existingRoleAction = companyRoleActionsRepository.Get(r => r.CompanyRoleId == roleAction.CompanyRoleId
                                                                && r.SystemPageActionId == roleAction.SystemPageActionId);

            if (existingRoleAction != null && existingRoleAction.ActionsInCompanyRolesId == roleAction.ActionsInCompanyRolesId)
            {
                return OperationState.Exists;
            }

            existingRoleAction.CompanyRoleId = roleAction.CompanyRoleId;
            existingRoleAction.SystemPageActionId = roleAction.SystemPageActionId;
            existingRoleAction.ModificationDate = DateTime.UtcNow;

            companyRoleActionsRepository.Update(existingRoleAction);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteRoleAction(Guid actionsInCompanyRolesId)
        {
            var roleAction = companyRoleActionsRepository.Get(g => g.ActionsInCompanyRolesId == actionsInCompanyRolesId);
            if (roleAction != null)
            {
                companyRoleActionsRepository.Delete(roleAction);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }


        public PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActionsByCompanyUserRoleId(CompanyRoleActionFilterByTextInputViewModel model)
        {
            var actions = companyRoleActionsRepository.GetMany(c => c.CompanyRoleId == model.CompanyRoleId);

            return GetPaged(model, actions);
        }

        public PagedOutput<CompanyRoleActionInfoViewModel> GetRoleActionsBySystemPageActionId(CompanyRoleActionFilterByTextInputViewModel model)
        {
            var actions = companyRoleActionsRepository.GetMany(c => c.SystemPageActionId == model.SystemPageActionId);

            return GetPaged(model, actions);
        }

        private PagedOutput<CompanyRoleActionInfoViewModel> GetPaged(CompanyRoleActionFilterByTextInputViewModel model, IEnumerable<ActionsInCompanyRoles> actions)
        {
            var output = new PagedOutput<CompanyRoleActionInfoViewModel>
            {
                TotalCount = actions.Count()
            };
            var result = companyRoleActionsRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<CompanyRoleActionInfoViewModel>>(result.ToList());

            return output;
        }

        public OperationState UpdatePagesActionsInRoles(UpdatePagesActionsInRoles model)
        {
            var oldRoleActions = companyRoleActionsRepository.GetMany(a => a.CompanyRoleId == model.PageActionsInRoles.First().CompanyRoleId);
            foreach (var item in oldRoleActions)
            {
                companyRoleActionsRepository.Delete(item);
            }
            foreach (var item in model.PageActionsInRoles)
            {
                companyRoleActionsRepository.Add(new ActionsInCompanyRoles
                {
                    CompanyRoleId = item.CompanyRoleId,
                    SystemPageActionId = item.SystemPageActionId,
                    CreationDate = DateTime.UtcNow
                });
            }
            var result = unitOfWork.Commit();
            return (result > 0) ? OperationState.Updated : OperationState.None;
        }
    }
}
