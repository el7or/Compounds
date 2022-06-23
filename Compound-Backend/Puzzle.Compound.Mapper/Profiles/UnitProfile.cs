using AutoMapper;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Units;
using System;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles {
	public class UnitProfile : Profile {
		public UnitProfile() {
			CreateMap<UnitInfoMap, UnitInfoViewModel>();

			CreateMap<CompoundUnit, UnitInfoViewModel>()
					.ForMember(x => x.CompoundId, cfg => cfg.MapFrom(z => z.CompoundGroup.Compound.CompoundId))
					.ForMember(x => x.CompoundGroupNameEn, cfg => cfg.MapFrom(z => z.CompoundGroup.NameEn))
					.ForMember(x => x.CompoundGroupNameAr, cfg => cfg.MapFrom(z => z.CompoundGroup.NameAr))
					.ForMember(x => x.CompoundTimeZone, cfg => cfg.MapFrom(z => z.CompoundGroup.Compound.TimeZoneText))
					.ForMember(x => x.CompoundTimeOffset, cfg => cfg.MapFrom(z => z.CompoundGroup.Compound.TimeZoneOffset))
					.ForMember(x => x.CompanyId, opt => opt.MapFrom(x => x.CompoundGroup.Compound.CompanyId))
					.ForMember(x => x.OwnersCount, opt => opt.MapFrom(x => x.OwnerUnits.Where(o => o.IsActive != null && o.IsActive.Value
																																																											&& o.IsDeleted != null && !o.IsDeleted.Value).Count()))
					.ForMember(x => x.SubOwnersCount, opt => opt.MapFrom(x => x.OwnerAssignedUnits.Where(o => !o.IsDeleted && o.IsActive
																																																											&& (o.EndTo == null || o.EndTo <= DateTime.UtcNow)).Count()));


			CreateMap<AddEditUnitViewModel, CompoundUnit>();

			CreateMap<UnitRequestInfoMap, UnitRequestInfoViewModel>().ReverseMap();

			CreateMap<OwnerAssignUnitRequest, AddUnitRequestViewModel>().ReverseMap();


			CreateMap<OwnerAssignedUnit, SubUserAssignUnitViewModel>()
					.ForMember(x => x.CompoundUnitName, cfg => cfg.MapFrom(z => z.CompoundUnit.Name))
					.ForMember(x => x.Compound_Unit_Id, cfg => cfg.MapFrom(z => z.CompoundUnitId))
					.ForMember(x => x.Start_From, cfg => cfg.MapFrom(z => z.StartFrom))
					.ForMember(x => x.End_To, cfg => cfg.MapFrom(z => z.EndTo))
					.ReverseMap()
					.ForMember(x => x.CompoundUnitId, cfg => cfg.MapFrom(z => z.Compound_Unit_Id))
					.ForMember(x => x.StartFrom, cfg => cfg.MapFrom(z => z.Start_From))
					.ForMember(x => x.EndTo, cfg => cfg.MapFrom(z => z.End_To));

			CreateMap<SubUserAssignUnitViewModel, OwnerUnit>()
				.ForMember(x => x.CompoundUnitId, cfg => cfg.MapFrom(z => z.Compound_Unit_Id));

		}
	}
}
