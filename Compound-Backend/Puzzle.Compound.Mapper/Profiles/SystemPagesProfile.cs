using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.CompanyRoleActions;
using Puzzle.Compound.Models.SystemPageActions;
using Puzzle.Compound.Models.SystemPages;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class SystemPagesProfile : Profile
    {
        public SystemPagesProfile()
        {
            CreateMap<SystemPage, AddEditSystemPageViewModel>().ReverseMap();
            CreateMap<SystemPage, SystemPageInfoViewModel>();

            CreateMap<SystemPageAction, AddEditSystemPageActionViewModel>().ReverseMap();
            CreateMap<SystemPageAction, SystemPageActionInfoViewModel>()
                .ForMember(a => a.IsSelected, cfg => cfg.MapFrom(s => s.ActionsInCompanyRoles.Any()));


            CreateMap<ActionsInCompanyRoles, AddEditCompanyRoleActionViewModel>().ReverseMap();
            CreateMap<ActionsInCompanyRoles, CompanyRoleActionInfoViewModel>();
        }
    }
}
