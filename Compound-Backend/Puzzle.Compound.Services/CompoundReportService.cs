using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.CompoundReports;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services {
	public interface ICompoundReportService {
		Task<CompoundReportOutput[]> GetCompoundsReports(CompoundReportInput compounds);
	}

	public class CompoundReportService : BaseService, ICompoundReportService {
		private readonly IReportTypeRepository _reportTypeRep;
		public CompoundReportService(IUnitOfWork unitOfWork, IMapper mapper,
			IReportTypeRepository reportTypeRep) : base(unitOfWork, mapper) {
			_reportTypeRep = reportTypeRep;
		}
		public async Task<CompoundReportOutput[]> GetCompoundsReports(CompoundReportInput compounds) {
			var services = await _reportTypeRep.Table.Include(x => x.CompoundReports).Where(x => x.IsFixed ||
				x.CompoundReports.Any(z => compounds.Compounds.Contains(z.CompoundId)))
				.ToListAsync();
			return mapper.Map<CompoundReportOutput[]>(services);
		}
	}
}
