using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Authorization;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Compounds;
using Puzzle.Compound.Models.Groups;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompoundsController : ControllerBase
    {
        private readonly ICompoundService compoundService;
        private readonly ICompoundGroupService groupService;
        private readonly IMapper mapper;
        private readonly IS3Service s3Service;
        private readonly ICompoundUnitService compoundUnitService;

        public CompoundsController(ICompoundService compoundService,
                        ICompoundGroupService groupService,
                        IMapper mapper,
                        IS3Service s3Service,
                        ICompoundUnitService compoundUnitService)
        {
            this.compoundService = compoundService;
            this.groupService = groupService;
            this.mapper = mapper;
            this.s3Service = s3Service;
            this.compoundUnitService = compoundUnitService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var compoundsList = await compoundService.GetCompoundsAsync();
            var mappedCompoundsList = mapper.Map<IEnumerable<CompoundViewModel>, IEnumerable<CompoundInfoViewModel>>(compoundsList);

            return Ok(new PuzzleApiResponse(result: mappedCompoundsList));
        }


        [HttpGet]
        [Route("{id}/all-groups")]
        public ActionResult GetCompoundAllGroups(Guid id)
        {
            var groups = groupService.GetGroupsByCompoundId(id);
            var mappedGroups = mapper.Map<IEnumerable<CompoundGroup>, IEnumerable<GroupInfoViewModel>>(groups);

            return Ok(new PuzzleApiResponse(result: mappedGroups));
        }

        [HttpGet]
        [Route("{id}/parent-groups")]
        public ActionResult GetCompoundParentGroups(Guid id)
        {
            var groups = groupService.GetGroupsByCompoundId(id, parentOnly: true);
            var mappedGroups = mapper.Map<IEnumerable<CompoundGroup>, IEnumerable<GroupInfoViewModel>>(groups);

            return Ok(new PuzzleApiResponse(result: mappedGroups));
        }

        [HttpGet]
        [Route("{id}/sub-groups")]
        public ActionResult GetCompoundSubGroups(Guid id)
        {
            var groups = groupService.GetGroupsByCompoundId(id, parentOnly: false);
            var mappedGroups = mapper.Map<IEnumerable<CompoundGroup>, IEnumerable<GroupInfoViewModel>>(groups);

            return Ok(new PuzzleApiResponse(result: mappedGroups));
        }

        [HttpGet("{id}")]
        public ActionResult GetById(Guid id)
        {
            var compound = compoundService.GetCompoundById(id);
            var mappedCompound = mapper.Map<Core.Models.Compound, CompoundInfoViewModel>(compound);

            return Ok(new PuzzleApiResponse(result: mappedCompound));
        }

        [HttpPost]
        public ActionResult Post(AddCompoundViewModel compoundData)
        {
            if (compoundData.Image != null && compoundData.Image.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "File should be less than or equal 2 MB!"));
            }

            var compoundInfo = mapper.Map<AddCompoundViewModel, Core.Models.Compound>(compoundData);

            var operationState = compoundService.AddCompound(compoundInfo);
            if (operationState == Common.Enums.OperationState.Created)
            {
                string logoUrl = "";

                if (compoundData.Image != null)
                {
                    // Upload Compound Logo
                    var fileBytes = Convert.FromBase64String(compoundData.Image.FileBase64);
                    string newFileName = "";

                    logoUrl = s3Service.UploadFile("compound", compoundData.Image.FileName, fileBytes, out newFileName);

                    if (!string.IsNullOrEmpty(logoUrl))
                    {
                        compoundInfo.Image = newFileName;
                        var logoUpdateOperation = compoundService.EditCompound(compoundInfo);
                    }
                }

                return Ok(new PuzzleApiResponse(result: new
                {
                    logo = logoUrl,
                    compoundId = compoundInfo.CompoundId
                }));
            }

            return Ok(new PuzzleApiResponse(message: "Compound can't be added"));
        }

        [HttpPut]
        public ActionResult Put(EditCompoundViewModel compoundData)
        {
            if (compoundData.Image != null && compoundData.Image.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "File should be less than or equal 2 MB!"));
            }

            var compoundInfo = mapper.Map<EditCompoundViewModel, Core.Models.Compound>(compoundData);


            string logoUrl = "";

            if (string.IsNullOrEmpty(compoundData.Image.Path))
            {
                // Upload Compound Logo
                var fileBytes = Convert.FromBase64String(compoundData.Image.FileBase64);
                string newFileName = "";

                logoUrl = s3Service.UploadFile("compound", compoundData.Image.FileName, fileBytes, out newFileName);
                if (!string.IsNullOrEmpty(logoUrl))
                {
                    compoundInfo.Image = newFileName;
                }
            }
            else
            {
                compoundInfo.Image = null;
                logoUrl = compoundData.Image.Path;
            }

            var operationState = compoundService.EditCompound(compoundInfo);
            if (operationState == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    logo = logoUrl,
                    compoundId = compoundInfo.CompoundId
                }));
            }

            return Ok(new PuzzleApiResponse(message: "Compound can't be updated"));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = compoundService.DeleteCompound(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Compound not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "Compound deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Compound can't be deleted"));
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult> Get([FromHeader] string Language, string name)
        {
            var compounds = await compoundService.GetCompounds(name, Language);
            return Ok(new PuzzleApiResponse(result: compounds));
        }

        [HttpGet("units")]
        public ActionResult GetCompoundUnits(Guid compoundId, string unitName)
        {
            unitName = unitName?.ToLower();
            var unitInfo = compoundUnitService.GetUnitsByCompoundId(compoundId, unitName);

            if (unitInfo == null)
            {
                return Ok(new PuzzleApiResponse(message: "Unit not found!"));
            }

            return Ok(new PuzzleApiResponse(result: unitInfo));
        }

        [HttpPost("dashboard")]
        public async Task<ActionResult> GetDashboardInfoAsync(DashboardFilterViewModel model)
        {
            dynamic dashboardInfo;
            if (model.ChartScope == null)
                dashboardInfo = await compoundService.GetDashboardInfoAsync(model);
            else
                dashboardInfo = compoundService.GetChartVisitsInfo(model.CompoundId, (ChartScope)model.ChartScope);

            return Ok(new PuzzleApiResponse(result: dashboardInfo));
        }

        [HttpPost("all-pendings")]
        public async Task<ActionResult> GetAllPendingsAsync(AllPendingsFilterViewModel model)
        {
               var allPendings = await compoundService.GetAllPendingsAsync(model);

            return Ok(new PuzzleApiResponse(result: allPendings));
        }

        [HttpPost("all-pendings-count")]
        public ActionResult GetAllPendingsCountAsync(AllPendingsFilterViewModel model)
        {
            var count = compoundService.GetAllPendingsCount(model);

            return Ok(new PuzzleApiResponse(result: count));
        }

        [HttpPost("emergency-call")]
        public async Task<ActionResult> PostEmergencyCallAsync(EmergencyCallViewModel model)
        {
            var result = await compoundService.PostEmergencyCall(model);

            return Ok(new PuzzleApiResponse(result: new
            {
                emergencyPhone = result,
            }));
        }
    }
}
