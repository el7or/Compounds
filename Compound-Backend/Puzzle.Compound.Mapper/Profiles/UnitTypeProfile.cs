using AutoMapper;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Models.UnitTypes;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class UnitTypeProfile : Profile
    {
        public UnitTypeProfile()
        {
            CreateMap<CompoundUnitType, UnitTypeInfoViewModel>();
        }
    }
}
