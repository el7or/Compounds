using AutoMapper;
using Puzzle.Compound.Data.Infrastructure;

namespace Puzzle.Compound.Services
{
    public class BaseService
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
    }
}
