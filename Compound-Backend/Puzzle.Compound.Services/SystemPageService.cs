using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.SystemPageActions;
using Puzzle.Compound.Models.SystemPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ISystemPageService
    {
        OperationState AddPage(AddEditSystemPageViewModel systemPage);
        OperationState EditPage(AddEditSystemPageViewModel systemPage);
        OperationState DeletePage(Guid systemPageId);
        SystemPageInfoViewModel GetPageById(Guid systemPageId);

        PagedOutput<SystemPageInfoViewModel> GetPages(SystemPageFilterByTextInputViewModel model);

        IEnumerable<SystemPageInfoViewModel> GetPagesByRole(Guid roleId);
        IEnumerable<SystemPageInfoViewModel> GetSubPages(Guid parentSystemPageId);
    }

    public class SystemPageService : BaseService, ISystemPageService
    {
        private readonly ISystemPageRepository systemPageRepository;

        public SystemPageService(ISystemPageRepository systemPageRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.systemPageRepository = systemPageRepository;
        }

        public OperationState AddPage(AddEditSystemPageViewModel systemPage)
        {
            if(systemPage.ParentPageId != null)
            {
                var parentGroup = GetPageById(systemPage.ParentPageId);
                if(parentGroup == null)
                {
                    return OperationState.NotExists;
                }
            }

            var existingPage = GetPageByName(systemPage.PageArabicName, systemPage.PageEnglishName);
            if(existingPage != null)
            {
                return OperationState.Exists;
            }
            
            var mappedPage = mapper.Map<AddEditSystemPageViewModel, SystemPage>(systemPage);
            systemPageRepository.Add(mappedPage);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                systemPage.SystemPageId = mappedPage.SystemPageId;
                return OperationState.Created;
            }
            else
            {
                return OperationState.None;
            }
        }

        public PagedOutput<SystemPageInfoViewModel> GetPages(SystemPageFilterByTextInputViewModel model)
        {
            var pages = systemPageRepository.GetMany(c => !c.IsDeleted && c.IsActive && (c.PageArabicName.ToLower().Contains(model.Text.ToLower())
                                                        || c.PageEnglishName.ToLower().Contains(model.Text.ToLower())));

            var output = new PagedOutput<SystemPageInfoViewModel>
            {
                TotalCount = pages.Count()
            };
            var result = systemPageRepository.Table.ApplyPaging(model);
            output.Result = mapper.Map<List<SystemPageInfoViewModel>>(result.ToList());

            return output;
        }

        public IEnumerable<SystemPageInfoViewModel> GetPagesByRole(Guid roleId)
        {
            var pagesActions = systemPageRepository.Table
                .Include(s => s.SystemPageActions)
                    .ThenInclude(a => a.ActionsInCompanyRoles.Where(r => r.CompanyRoleId == roleId));

            return mapper.Map<List<SystemPageInfoViewModel>>(pagesActions.ToList());
        }

        public SystemPageInfoViewModel GetPageById(Guid id)
        {
            var page = GetMainPageById(id);
            return mapper.Map<SystemPage, SystemPageInfoViewModel>(page);
        }

        public SystemPage GetMainPageById(Guid id)
        {
            return systemPageRepository.Get(g => g.SystemPageId == id
                                        && !g.IsDeleted
                                            && g.IsActive);
        }

        public SystemPage GetPageByName(string name)
        {
            return systemPageRepository.Get(c => ( c.PageEnglishName.ToLower().Contains(name.ToLower()) || c.PageArabicName.ToLower().Contains(name.ToLower())) && c.IsDeleted == false);
        }

        public SystemPage GetPageByName(string nameAr, string nameEn)
        {
            return systemPageRepository.Get(c => (c.PageEnglishName.ToLower().Contains(nameEn.ToLower()) || c.PageArabicName.ToLower().Contains(nameAr.ToLower())) && c.IsDeleted == false);
        }

        public OperationState EditPage(AddEditSystemPageViewModel updatedPage)
        {
            var existingPage = GetMainPageById(updatedPage.SystemPageId);
            
            if (existingPage.ParentPageId != updatedPage.ParentPageId)
            {
                if (updatedPage.ParentPageId != null)
                {
                    var parentGroup = GetPageById(updatedPage.ParentPageId);
                    if (parentGroup == null)
                    {
                        return OperationState.NotExists;
                    }
                }
            }

            existingPage = GetPageByName(updatedPage.PageArabicName, updatedPage.PageEnglishName);
            if (existingPage != null && existingPage.SystemPageId == updatedPage.SystemPageId)
            {
                return OperationState.Exists;
            }

            existingPage.ParentPageId = updatedPage.ParentPageId;
            existingPage.PageArabicName = updatedPage.PageArabicName;
            existingPage.PageEnglishName = updatedPage.PageEnglishName;
            existingPage.PageIndex = updatedPage.PageIndex;
            existingPage.PageURL = updatedPage.PageURL;

            systemPageRepository.Update(existingPage);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeletePage(Guid systemPageId)
        {
            var page = systemPageRepository.Get(g => g.SystemPageId == systemPageId);
            if (page != null)
            {
                page.IsDeleted = true;
                page.IsActive = false;
                systemPageRepository.Update(page);
                var result = unitOfWork.Commit();

                return result > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public IEnumerable<SystemPageInfoViewModel> GetSubPages(Guid parentPageId)
        {
            var pages = systemPageRepository.GetMany(g => g.ParentPageId == parentPageId
                                                        && !g.IsDeleted
                                                        && g.IsActive);

            return mapper.Map<IEnumerable<SystemPage>, IEnumerable<SystemPageInfoViewModel>>(pages);
        }
    }
}
