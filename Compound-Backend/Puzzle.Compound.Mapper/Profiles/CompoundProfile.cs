using AutoMapper;
using Newtonsoft.Json;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Companies;
using Puzzle.Compound.Models.Compounds;

namespace Puzzle.Compound.Mapper.Profiles {
	public class CompoundProfile : Profile {
		public CompoundProfile() {
			// TODO Get it from configuration
			string s3Url = "http://circle360.s3.amazonaws.com/";

			CreateMap<Core.Models.Compound, CompoundInfoViewModel>()
					.ForMember(c =>
										 c.Location,
										 opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)))
					.ForMember(c =>
											c.Image,
											opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Image) ? "" : s3Url + src.Image)).ReverseMap();

			CreateMap<CompoundViewModel, CompoundInfoViewModel>()
					.ForMember(c =>
										 c.Location,
										 opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)))
					.ForMember(c =>
											c.Image,
											opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Image) ? "" : s3Url + src.Image)).ReverseMap();

			CreateMap<Core.Models.Compound, AddCompoundViewModel>()
					.ForMember(c =>
										 c.Location,
										 opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)));

			CreateMap<AddCompoundViewModel, Core.Models.Compound>()
					.ForMember(c =>
										 c.Location,
										 opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Location)));

			CreateMap<EditCompoundViewModel, Core.Models.Compound>()
					.ForMember(c =>
										 c.Location,
										 opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Location)));
			CreateMap<Core.Models.Compound, CompanyCompound>();
		}
	}
}
