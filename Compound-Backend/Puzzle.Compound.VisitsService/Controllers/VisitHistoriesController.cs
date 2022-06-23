using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.VisitTransactionHistory;
using Puzzle.Compound.Services;
using System.IO;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VisitHistoriesController : ControllerBase
    {
        private readonly IVisitTranscationHistoryService _visitTranscationHistoryService;
        private readonly IS3Service _s3Service;

        public VisitHistoriesController(IVisitTranscationHistoryService visitTranscationHistoryService,
                        IS3Service s3Service)
        {
            _visitTranscationHistoryService = visitTranscationHistoryService;
            _s3Service = s3Service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] VisitTransactionHistoryFilterViewModel model)
        {
            if (model.Status == VisitStatus.Pending)
            {
                var result = await _visitTranscationHistoryService.GetPendingVisitsAsync(model);
                return Ok(result);
            }
            else
            {
                var result = await _visitTranscationHistoryService.GetAsync(model);
                return Ok(result);
            }
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ExportExcelAsync([FromBody] VisitTransactionHistoryFilterViewModel model,
            [FromHeader] int timezone,
           [FromHeader] string language)
        {
            // get all visits
            model.PageNumber = 1;
            model.PageSize = 9999;

            VisitTransactionHistoryPagedOutput visitsList = new VisitTransactionHistoryPagedOutput();
            if (model.Status == VisitStatus.Pending)
            {
                visitsList = await _visitTranscationHistoryService.GetPendingVisitsAsync(model);
            }
            else
            {
                visitsList = await _visitTranscationHistoryService.GetAsync(model);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var xlsSheet = package.Workbook.Worksheets.Add("Visits");
                xlsSheet.Cells["A1"].Value = "#";
                xlsSheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                xlsSheet.Cells["B1"].Value = language == "en" ? "Visit Requester" : "طالب الزيارة";
                xlsSheet.Cells["C1"].Value = language == "en" ? "Unit Name" : "اسم الوحدة";
                xlsSheet.Cells["D1"].Value = language == "en" ? "Gate Name" : "اسم البوابة";
                xlsSheet.Cells["E1"].Value = language == "en" ? "Visit Date & Time" : "تاريخ ووقت الزيارة";
                xlsSheet.Cells["F1"].Value = language == "en" ? "Visit Type" : "نوع الزيارة";
                xlsSheet.Cells["G1"].Value = language == "en" ? "Visit Status" : "حالة الزيارة";
                xlsSheet.Cells["A1:G1"].Style.Font.Bold = true;
                xlsSheet.Cells["A1:G4"].Style.ShrinkToFit = false;

                int i = 2;
                foreach (var visit in visitsList.Result)
                {
                    xlsSheet.Cells["A" + i].Value = i - 1;
                    xlsSheet.Cells["A" + i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    xlsSheet.Cells["B" + i].Value = visit.OwnerName;
                    xlsSheet.Cells["C" + i].Value = visit.UnitName;
                    xlsSheet.Cells["D" + i].Value = visit.GateName;
                    if (model.Status != VisitStatus.Pending)
                    {
                        var dateTimeZone = visit.Date.Value.AddHours(timezone);
                        xlsSheet.Cells["E" + i].Value = dateTimeZone.ToShortDateString() + " " + dateTimeZone.ToShortTimeString();
                    }
                    xlsSheet.Cells["F" + i].Value = language == "en" ? visit.Type.ToString() : visit.Type.GetDescription();
                    xlsSheet.Cells["G" + i].Value = language == "en" ? visit.Status.ToString() : visit.Status.GetDescription();
                    i++;
                }

                var excelData = package.GetAsByteArray();
                var fileUrl = _s3Service.UploadFile("Visits", "visits-list.xlsx", excelData, sameFileName: true);
                return Ok(fileUrl);
            }
        }

        [HttpPost("filterByUser/card")]
        [ProducesResponseType(typeof(PagedOutput<VisitTransactionHistoryFilterByUserOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByUserCard(PagedInput model)
        {
            var result = await _visitTranscationHistoryService.FilterByUserCardAsync(model);
            return Ok(result);
        }
    }
}
