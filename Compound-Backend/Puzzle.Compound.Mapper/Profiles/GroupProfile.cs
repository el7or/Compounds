using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Groups;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {

            CreateMap<CompoundGroup, GroupInfoViewModel>()
                .ForMember(x => x.SubGroupsCount, opt => opt.MapFrom(x => x.Groups.Where(x => x.IsActive.Value && !x.IsDeleted.Value).Count()))
                .ForMember(x => x.UnitsCount, opt => opt.MapFrom(x => x.CompoundUnits.Where(x => x.IsActive.Value && !x.IsDeleted.Value).Count()));

            CreateMap<AddEditCompoundGroupViewModel, CompoundGroup>();

            CreateMap<CompoundUnit, GroupUnitViewModel>();

        }
    }
}
