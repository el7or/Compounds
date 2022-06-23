using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Owners;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles {
	public class OwnerProfile : Profile {
		public OwnerProfile() {

			CreateMap<OwnerRegistration, OwnerRegisterViewModel>()
				.ForMember(x => x.Units, cfg => cfg.MapFrom(z => z.OwnerAssignedUnits.Where(z => z.IsDeleted != true)));
			CreateMap<OwnerRegisterViewModel, OwnerRegistration>();

			CreateMap<AddOwnerSubUserViewModel, OwnerRegistration>().ReverseMap();

			CreateMap<CompoundOwner, OwnerRegisterViewModel>().ReverseMap();

			CreateMap<CompoundOwner, OwnerInfoViewModel>()
				.ForMember(x => x.UnitsIds, cfg => cfg.MapFrom(z => z.OwnerUnits.Select(u => u.CompoundUnitId).ToList()))
				.ForMember(x => x.UnitsNames, cfg => cfg.MapFrom(z => z.OwnerUnits.Select(u => u.CompoundUnit.Name).ToList()));

			CreateMap<CompoundOwner, OwnerInputViewModel>().ReverseMap();

			CreateMap<CompoundOwner, OwnerRegistration>().ReverseMap();


			CreateMap<OwnerInfoViewModel, OwnerRegistration>().ReverseMap();

			CreateMap<OwnerRegistration, LoginUserInfo>();
		}
	}
}
