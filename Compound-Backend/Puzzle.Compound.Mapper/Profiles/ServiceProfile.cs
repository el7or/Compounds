using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CompoundServiceModel, CompoundService>()
                .ForMember(z => z.Order, cfg => cfg.MapFrom(y => y.AssignOrder));

            CreateMap<ServiceRequestModel, ServiceRequest>()
                .ForMember(x => x.CompoundUnitId, cfg => cfg.MapFrom(z => z.UnitId));

            CreateMap<ServiceRequestSubTypeInPutViewModel, ServiceRequestSubType>();

            CreateMap<ServiceRequest, OwnerServiceViewModel>();

            CreateMap<ServiceUpdateModel, ServiceRequest>();

            CreateMap<ServiceType, CompoundServiceOutput>();

            CreateMap<ServiceSubTypeInputViewModel, ServiceSubType>();
            CreateMap<ServiceSubType, ServiceSubTypeOutputViewModel>();
        }
    }
}
