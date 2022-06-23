using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    public class PuzzleBaseController : ControllerBase
    {
        public PuzzleBaseController(IMapper mapper)
        {
            Mapper = mapper;
        }

        public IMapper Mapper { get; set; }
    }
}
