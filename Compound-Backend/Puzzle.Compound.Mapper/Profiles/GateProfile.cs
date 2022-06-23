using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Gates;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles {
	public class GateProfile : Profile {
		public GateProfile() {
			CreateMap<GateAddViewModel, Gate>();
			CreateMap<Gate, GateOutputViewModel>()
					.ForMember(x => x.CompoundIds, opt => opt.MapFrom(x => x.CompoundGates.Where(x => x.IsActive.Value && !x.IsDeleted.Value).Select(x => x.CompoundId)))
					.ForMember(x => x.UserName, cfg => cfg.MapFrom(z => z.User.Username))
					.ForMember(x => x.Password, cfg => cfg.MapFrom(z => z.User.Password))
					.ForMember(x => x.UserId, cfg => cfg.MapFrom(z => z.User.CompanyUserId));
			CreateMap<GateUpdateViewModel, Gate>()
					.IncludeBase<GateAddViewModel, Gate>();
			CreateMap<Gate, GateFilterOutputViewModel>();
			CreateMap<Gate, GateListModel>();
			CreateMap<Gate, GateLoginModel>()
				.ForMember(x => x.UserId, cfg => cfg.MapFrom(z => z.User.CompanyUserId))
				.ForMember(x => x.TimezoneOffset, cfg => cfg.MapFrom(z => z.CompoundGates
						 .FirstOrDefault(g => g.IsDeleted != true).Compound.TimeZoneOffset))
				.ForMember(x => x.TimezoneValue, cfg => cfg.MapFrom(z => z.CompoundGates
						 .FirstOrDefault(g => g.IsDeleted != true).Compound.TimeZoneValue));
		}
	}
}
