using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<CompoundNews, NewsOutputViewModel>();

            CreateMap<CompoundNewsImage, PuzzleFileInfo>()
            .ForMember(i => i.SizeInBytes, opt => opt.Ignore());

            CreateMap<NewsInputViewModel, CompoundNews>()
                .ForMember(i => i.Images, opt => opt.Ignore());
        }
    }
}
