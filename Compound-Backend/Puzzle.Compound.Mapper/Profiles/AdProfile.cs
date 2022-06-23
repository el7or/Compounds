using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Ads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class AdProfile : Profile
    {
        public AdProfile()
        {
            CreateMap<CompoundAd, AdOutputViewModel>()
                .ForMember(x => x.IsUrl, cfg => cfg.MapFrom(ad => !string.IsNullOrEmpty(ad.AdUrl)))
                .ForMember(x => x.ShowsCount, cfg => cfg.MapFrom(ad => ad.CompoundAdHistories.Count(h => h.ActionType == ActionType.Show)))
                .ForMember(x => x.ClicksCount, cfg => cfg.MapFrom(ad => ad.CompoundAdHistories.Count(h => h.ActionType == ActionType.Click)))
                .ForMember(x => x.UniqueShowsCount, cfg => cfg.MapFrom(ad => ad.CompoundAdHistories.Where(h => h.ActionType == ActionType.Show).GroupBy(ad => ad.OwnerRegistrationId).Count()))
                .ForMember(x => x.UniqueClicksCount, cfg => cfg.MapFrom(ad => ad.CompoundAdHistories.Where(h => h.ActionType == ActionType.Click).GroupBy(ad => ad.OwnerRegistrationId).Count()));

            CreateMap<AdInputViewModel, CompoundAd>()
                .ForMember(i => i.Images, opt => opt.Ignore());

            CreateMap<CompoundAdImage, PuzzleFileInfo>()
            .ForMember(i => i.SizeInBytes, opt => opt.Ignore());
        }
    }
}
