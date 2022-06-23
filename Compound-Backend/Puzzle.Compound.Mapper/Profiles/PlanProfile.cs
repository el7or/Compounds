using AutoMapper;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Plans;
using Puzzle.Compound.Models.Units;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class PlanProfile : Profile
    {
        public PlanProfile()
        {
            CreateMap<Plan, PlanViewModel>().ReverseMap();
            CreateMap<Plan, AddPlanViewModel>().ReverseMap();
            CreateMap<Plan, PlanInfoViewModel>().ReverseMap();
        }
    }
}
