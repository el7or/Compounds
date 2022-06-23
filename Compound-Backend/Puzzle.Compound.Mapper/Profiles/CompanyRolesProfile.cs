using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.CompanyRoles;
using Puzzle.Compound.Models.CompanyUserRoles;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class CompanyRolesProfile : Profile
    {
        public CompanyRolesProfile()
        {
            CreateMap<CompanyRole, AddEditCompanyRoleViewModel>().ReverseMap();
            CreateMap<CompanyRole, CompanyRoleInfoViewModel>()
                .ForMember(x => x.PagesCount, cfg => cfg.MapFrom(n => n.ActionsInCompanyRoles.Count))
                .ForMember(x => x.UsersCount, cfg => cfg.MapFrom(n => n.CompanyUserRoles.Count));
            CreateMap<CompanyRole, CompanyCurrentRoleViewModel>();

            CreateMap<CompanyUserRole, AddEditCompanyUserRoleViewModel>().ReverseMap();
            CreateMap<CompanyUserRole, CompanyUserRoleInfoViewModel>();
        }
    }
}
