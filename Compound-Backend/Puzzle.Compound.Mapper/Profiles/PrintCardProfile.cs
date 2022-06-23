using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.PrintCardRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class PrintCardProfile : Profile
    {
        public PrintCardProfile()
        {
            CreateMap<PrintCardAddViewModel, PrintCardRequest>()
                .ForMember(x => x.Picture, opt => opt.Ignore());
            CreateMap<PrintCardRequest, PrintCardOutputViewModel>()
                .ForMember(x => x.OwnerName, opt => opt.MapFrom(x => x.OwnerRegistration.Name))
                .ForMember(x => x.QrCode, opt => opt.MapFrom(x => x.VisitRequestId == null ? null : x.VisitRequest.QrCode))
                .ForMember(x => x.Code, opt => opt.MapFrom(x => x.VisitRequestId == null ? null : x.VisitRequest.Code))
                .ForMember(x => x.UnitName, opt => opt.MapFrom(x => x.CompoundUnit.Name));
        }
    }
}
