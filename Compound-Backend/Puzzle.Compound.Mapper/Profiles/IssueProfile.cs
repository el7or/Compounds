using AutoMapper;
using Newtonsoft.Json;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Issues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<CompoundIssueModel, CompoundIssue>()
                .ForMember(z => z.Order, cfg => cfg.MapFrom(y => y.AssignOrder));

            CreateMap<IssueRequestModel, IssueRequest>()
                .ForMember(c => c.Location, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Location)));

            CreateMap<IssueUpdateModel, IssueRequest>()
                .ForMember(c => c.Location, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Location)));

            CreateMap<IssueRequest, OwnerIssueViewModel>()
                .ForMember(c => c.Location, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Location>(src.Location)));

            CreateMap<IssueType, CompoundIssueOutput>();
        }
    }
}
