using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Plans;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Services
{
    public interface IPlanService
    {
        OperationState AddPlan(AddPlanViewModel plan);
        Plan EditPlan(Plan plan);
        string DeletePlan(Guid planId);
        PlanInfoViewModel GetPlanById(Guid id);
        Plan GetPlanByName(string nameAr, string nameEn);
        IEnumerable<PlanInfoViewModel> GetPlans();
    }

    public class PlanService : BaseService, IPlanService
    {
        private readonly IPlanRepository planRepository;

        public PlanService(IPlanRepository planRepository, IUnitOfWork unitOfWork, IMapper mapper) :
            base(unitOfWork, mapper)
        {
            this.planRepository = planRepository;
        }

        public OperationState AddPlan(AddPlanViewModel plan)
        {
            var existsPlan = GetPlanByName(plan.PlanNameAr, plan.PlanNameEn);
            if(existsPlan != null)
            {
                return OperationState.Exists;
            }

            var mappedPlan = mapper.Map<AddPlanViewModel, Plan>(plan);
            mappedPlan.IsDeleted = false;
            mappedPlan.IsActive = true;

            planRepository.Add(mappedPlan);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Created : OperationState.None;
        }

        public IEnumerable<PlanInfoViewModel> GetPlans()
        {
            var plans = planRepository.GetMany(c => c.IsDeleted == false);
            return mapper.Map<IEnumerable<Plan>, IEnumerable<PlanInfoViewModel>>(plans);
        }

        public PlanInfoViewModel GetPlanById(Guid id)
        {
            var plan = GetPlan(id);
            return mapper.Map<Plan, PlanInfoViewModel>(plan);
        }

        private Plan GetPlan(Guid id)
        {
            return planRepository.Get(c => c.PlanId == id && c.IsDeleted == false);
        }

        public Plan GetPlanByName(string nameAr, string nameEn)
        {
            return planRepository.Get(c => ( c.PlanNameAr.Contains(nameAr) || c.PlanNameEn.Contains(nameEn)) && c.IsDeleted == false);
        }

        public Plan EditPlan(Plan plan)
        {
            planRepository.Update(plan);
            int result = unitOfWork.Commit();

            if (result > 0)
                return plan;
            return null;
        }

        public string DeletePlan(Guid planId)
        {
            var plan = GetPlan(planId);
            if (plan != null)
            {
                plan.IsDeleted = true;
                planRepository.Update(plan);
                if (unitOfWork.Commit() > 0)
                    return planId.ToString();
            }
            return "";
        }
    }
}
