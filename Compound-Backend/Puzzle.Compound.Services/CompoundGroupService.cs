using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Services
{
    public interface ICompoundGroupService
    {
        OperationState AddGroup(AddEditCompoundGroupViewModel group);
        OperationState EditGroup(AddEditCompoundGroupViewModel group);
        OperationState DeleteGroup(Guid groupId);
        GroupInfoViewModel GetGroupById(Guid id);

        IEnumerable<CompoundGroup> GetGroupsByCompoundId(Guid compoundId, bool? parentOnly = null);
        IEnumerable<CompoundGroup> GetGroupsByCompoundId(Guid compoundId, string nameAr, string nameEn);
        CompoundGroup GetGroupByName(string name);
        IEnumerable<CompoundGroup> GetGroups();
        IEnumerable<GroupInfoViewModel> GetSubGroups(Guid parentGroupId);
    }

    public class CompoundGroupService : BaseService, ICompoundGroupService
    {
        private readonly ICompoundGroupRepository groupRepository;

        public CompoundGroupService(ICompoundGroupRepository groupRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.groupRepository = groupRepository;
        }

        public OperationState AddGroup(AddEditCompoundGroupViewModel group)
        {
            var exitingGroups = GetGroupsByCompoundId(group.CompoundId, group.NameAr, group.NameEn);

            if (exitingGroups.Count() > 0)
            {
                return OperationState.Exists;
            }

            if(group.ParentGroupId != null)
            {
                var parentGroup = GetGroupById(group.ParentGroupId.Value);
                if(parentGroup == null)
                {
                    return OperationState.NotExists;
                }
            }
            
            var mappedGroup = mapper.Map<AddEditCompoundGroupViewModel, CompoundGroup>(group);
            mappedGroup.CreationDate = DateTime.UtcNow;
            groupRepository.Add(mappedGroup);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                group.CompoundGroupId = mappedGroup.CompoundGroupId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public IEnumerable<CompoundGroup> GetGroups()
        {
            return groupRepository.GetMany(c => c.IsDeleted == false);
        }

        public GroupInfoViewModel GetGroupById(Guid id)
        {
            var group = GetMainGroupById(id);
            return mapper.Map<CompoundGroup, GroupInfoViewModel>(group);
        }

        public CompoundGroup GetMainGroupById(Guid id)
        {
            return groupRepository.Get(g => g.CompoundGroupId == id
                                        && g.IsDeleted != null && !g.IsDeleted.Value
                                            && g.IsActive != null && g.IsActive.Value);
        }

        public CompoundGroup GetGroupByName(string name)
        {
            return groupRepository.Get(c => ( c.NameEn.ToLower().Contains(name.ToLower()) || c.NameAr.ToLower().Contains(name.ToLower())) && c.IsDeleted == false);
        }

        public OperationState EditGroup(AddEditCompoundGroupViewModel updatedGroup)
        {
            var existingGroup = GetMainGroupById(updatedGroup.CompoundGroupId.Value);
            var exitingGroups = GetGroupsByCompoundId(updatedGroup.CompoundId, updatedGroup.NameAr, updatedGroup.NameEn);
            exitingGroups = exitingGroups.Where(g => g.CompoundGroupId != updatedGroup.CompoundGroupId);

            if (exitingGroups.Count() > 0)
            {
                return OperationState.Exists;
            }

            if (existingGroup.ParentGroupId != updatedGroup.ParentGroupId)
            {
                if (updatedGroup.ParentGroupId != null)
                {
                    var parentGroup = GetGroupById(updatedGroup.ParentGroupId.Value);
                    if (parentGroup == null)
                    {
                        return OperationState.NotExists;
                    }
                }
            }

            existingGroup.ParentGroupId = updatedGroup.ParentGroupId;
            existingGroup.NameAr = updatedGroup.NameAr;
            existingGroup.NameEn = updatedGroup.NameEn;

            groupRepository.Update(existingGroup);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteGroup(Guid groupId)
        {
            var group = groupRepository.Get(g => g.CompoundGroupId == groupId);
            if (group != null)
            {
                if(group.CompoundUnits?.Count > 0)
                {
                    return OperationState.None;
                }

                group.IsDeleted = true;
                group.IsActive = false;
                groupRepository.Update(group);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public IEnumerable<CompoundGroup> GetGroupsByCompoundId(Guid compoundId, string nameAr, string nameEn)
        {
            return groupRepository.GetMany(g => g.CompoundId == compoundId && g.IsDeleted != null && !g.IsDeleted.Value
                                            && g.IsActive != null && g.IsActive.Value
                                            && (string.IsNullOrEmpty(nameAr) ||
                                                g.NameAr.ToLower().Contains(nameAr.ToLower()))
                                            && (string.IsNullOrEmpty(nameEn) ||
                                                g.NameEn.ToLower().Contains(nameEn.ToLower())));
        }

        public IEnumerable<CompoundGroup> GetGroupsByCompoundId(Guid compoundId, bool? parentOnly = null)
        {
            return groupRepository.GetMany(g => g.CompoundId == compoundId && g.IsDeleted != null && !g.IsDeleted.Value
                                            && g.IsActive != null && g.IsActive.Value
                                            && (parentOnly ==null || (parentOnly.Value?g.ParentGroupId == null:g.ParentGroupId != null)));
        }

        public IEnumerable<GroupInfoViewModel> GetSubGroups(Guid parentGroupId)
        {
            var groups = groupRepository.GetMany(g => g.ParentGroupId == parentGroupId
                                            && g.IsDeleted != null && !g.IsDeleted.Value
                                            && g.IsActive != null && g.IsActive.Value);

            return mapper.Map<IEnumerable<CompoundGroup>, IEnumerable<GroupInfoViewModel>>(groups);
        }
    }
}
