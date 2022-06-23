using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.CompanyUsers;
using System.Linq;

namespace Puzzle.Compound.Mapper.Profiles {
	public class CompanyUserProfile : Profile {
		public CompanyUserProfile() {
			// TODO Get it from configuration
			string s3Url = "http://circle360.s3.amazonaws.com/";

			CreateMap<CompanyUser, CompanyUserViewModel>().ReverseMap();
			CreateMap<CompanyUserInfoViewModel, CompanyUser>()
				.ForMember(z => z.CompanyUserCompounds, cfg => cfg.Ignore())
				.ReverseMap()
				.ForMember(x => x.UserImage, cfg => cfg.MapFrom(y => new PuzzleFileInfo { Path = s3Url + y.Image }));
			CreateMap<CompanyUserCompound, CompoundUserInfo>()
				.ForMember(z => z.Services, cfg => cfg.MapFrom(x => x.CompanyUserServices.Select(y => y.ServiceTypeId).ToArray()))
				.ForMember(z => z.Issues, cfg => cfg.MapFrom(x => x.CompanyUserIssues.Select(y => y.IssueTypeId).ToArray()));
			CreateMap<CompanyUserCompound, CompoundUserInfo>()
				.ForMember(z => z.Services, cfg => cfg.MapFrom(d => d.CompanyUserServices.Select(y => y.ServiceTypeId)))
				.ForMember(z => z.Issues, cfg => cfg.MapFrom(d => d.CompanyUserIssues.Select(y => y.IssueTypeId)));
		}
	}
}
