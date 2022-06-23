using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Services
{
    public interface ICompoundUnitTypeService
    {
        CompoundUnitType AddUnitType(CompoundUnitType unitType);
        CompoundUnitType EditUnitType(CompoundUnitType unitType);
        string DeleteUnitType(int id);
        CompoundUnitType GetUnitTypeById(int id);
        CompoundUnitType GetUnitTypeByName(string name);
        IEnumerable<CompoundUnitType> GetUnitTypes();
    }

    public class CompoundUnitTypeService : BaseService, ICompoundUnitTypeService
    {
        private readonly ICompoundUnitTypeRepository unitTypeRepository;

        public CompoundUnitTypeService(ICompoundUnitTypeRepository unitTypeRepository, IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            this.unitTypeRepository = unitTypeRepository;
        }

        public CompoundUnitType AddUnitType(CompoundUnitType unitType)
        {
            unitTypeRepository.Add(unitType);
            int result = unitOfWork.Commit();

            if (result > 0)
                return unitType;
            return null;
        }

        public IEnumerable<CompoundUnitType> GetUnitTypes()
        {
            return unitTypeRepository.GetMany(c => c.IsDeleted == false);
        }

        public CompoundUnitType GetUnitTypeById(int id)
        {
            return unitTypeRepository.Get(c => c.CompoundUnitTypeId == id && c.IsDeleted == false);
        }

        public CompoundUnitType GetUnitTypeByName(string name)
        {
            return unitTypeRepository.Get(c => ( c.NameEn.Contains(name) || c.NameAr.Contains(name)) && c.IsDeleted == false);
        }

        public CompoundUnitType EditUnitType(CompoundUnitType unitType)
        {
            unitTypeRepository.Update(unitType);
            int result = unitOfWork.Commit();

            if (result > 0)
                return unitType;
            return null;
        }

        public string DeleteUnitType(int id)
        {
            var unitType = GetUnitTypeById(id);
            if(unitType != null)
            {
                unitType.IsDeleted = true;
                unitTypeRepository.Update(unitType);
                if (unitOfWork.Commit() > 0)
                    return id.ToString();
            }
            return "";
        }
    }
}
