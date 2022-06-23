using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.SystemPageActions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Services
{
    public interface ISystemPageActionService
    {
        OperationState AddAction(AddEditSystemPageActionViewModel action);
        OperationState EditAction(AddEditSystemPageActionViewModel action);
        OperationState DeleteAction(Guid actionId);
        SystemPageActionInfoViewModel GetActionById(Guid actionId);
        PagedOutput<SystemPageActionInfoViewModel> GetActions(SystemPageActionFilterByTextInputViewModel model);
        PagedOutput<SystemPageActionInfoViewModel> GetActionsByPageId(SystemPageActionFilterByTextInputViewModel model);
    }

    public class SystemPageActionService : BaseService, ISystemPageActionService
    {
        private readonly ISystemPageActionRepository systemPageActionRepository;

        public SystemPageActionService(ISystemPageActionRepository systemPageActionRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.systemPageActionRepository = systemPageActionRepository;
        }

        public OperationState AddAction(AddEditSystemPageActionViewModel action)
        {
            var existingAction = GetActionByUniqueName(action.ActionUniqueName);
            if(existingAction != null)
            {
                return OperationState.Exists;
            }

            var mappedAction = mapper.Map<AddEditSystemPageActionViewModel, SystemPageAction>(action);
            systemPageActionRepository.Add(mappedAction);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                action.SystemPageActionId = mappedAction.SystemPageActionId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public PagedOutput<SystemPageActionInfoViewModel> GetActions(SystemPageActionFilterByTextInputViewModel model)
        {
            var pages = systemPageActionRepository.GetMany(r => r.ActionArabicName.ToLower().Contains(model.Text.ToLower())
                                                        || r.ActionEnglishName.ToLower().Contains(model.Text.ToLower()));

            var output = new PagedOutput<SystemPageActionInfoViewModel>
            {
                TotalCount = pages.Count()
            };
            var result = systemPageActionRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<SystemPageActionInfoViewModel>>(result.ToList());

            return output;
        }

        public SystemPageActionInfoViewModel GetActionById(Guid id)
        {
            var action = systemPageActionRepository.Get(g => g.SystemPageActionId == id);
            return mapper.Map<SystemPageAction, SystemPageActionInfoViewModel>(action);
        }

        public SystemPageAction GetActionByUniqueName(string uniqueActionName)
        {
            return systemPageActionRepository.Get(c => c.ActionUniqueName.ToLower().Contains(uniqueActionName.ToLower()));
        }

        public OperationState EditAction(AddEditSystemPageActionViewModel updatedAction)
        {
            var existingAction = systemPageActionRepository.Get(g => g.SystemPageActionId == updatedAction.SystemPageActionId);
            
            existingAction = GetActionByUniqueName(updatedAction.ActionUniqueName);
            if (existingAction != null && existingAction.SystemPageActionId == updatedAction.SystemPageActionId)
            {
                return OperationState.Exists;
            }

            existingAction.ActionArabicName = updatedAction.ActionArabicName;
            existingAction.ActionEnglishName = updatedAction.ActionEnglishName;
            existingAction.ActionUniqueName = updatedAction.ActionUniqueName;

            systemPageActionRepository.Update(existingAction);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteAction(Guid actionId)
        {
            var page = systemPageActionRepository.Get(g => g.SystemPageActionId == actionId);
            if (page != null)
            {
                systemPageActionRepository.Delete(page);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public PagedOutput<SystemPageActionInfoViewModel> GetActionsByPageId(SystemPageActionFilterByTextInputViewModel model)
        {
            var actions = systemPageActionRepository.GetMany(c => c.SystemPageId == model.SystemPageId);

            var output = new PagedOutput<SystemPageActionInfoViewModel>
            {
                TotalCount = actions.Count()
            };
            var result = systemPageActionRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<SystemPageActionInfoViewModel>>(result.ToList());

            return output;
        }
    }
}
