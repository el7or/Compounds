using AutoMapper;
using Newtonsoft.Json;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Companies;
using Puzzle.Compound.Models.CompoundReports;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // TODO Get it from configuration
            string s3Url = "http://circle360.s3.amazonaws.com/";

            CreateMap<Company, AddCompanyViewModel>()
                    .ForMember(c =>
                                         c.Location,
                                         opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)))
                    .ForMember(c =>
                                            c.PlanId,
                                            opt => opt.MapFrom(src => src.PlanId));

            CreateMap<AddCompanyViewModel, Company>()
                    .ForMember(c =>
                                         c.Location,
                                         opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Location)))
                    .ForMember(c =>
                                            c.PlanId,
                                            opt => opt.MapFrom(src => src.PlanId));

            CreateMap<Company, CompanyInfoViewModel>()
                    .ForMember(c =>
                                         c.Location,
                                         opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)))
                    .ForMember(c =>
                                            c.Plan,
                                            opt => opt.MapFrom(src => src.Plan))
                    .ForMember(c =>
                                            c.Logo,
                                            opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Logo) ? "" : s3Url + src.Logo)).ReverseMap();


            CreateMap<Company, CompanyInfo2ViewModel>().ReverseMap();

            CreateMap<ReportType, CompoundReportOutput>();
        }
    }
}
