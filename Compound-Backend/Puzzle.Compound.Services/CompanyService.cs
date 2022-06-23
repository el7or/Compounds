using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Companies;
using Puzzle.Compound.Models.Issues;
using Puzzle.Compound.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompanyService
    {
        OperationState AddCompany(Company company);
        Task<OperationState> AddCompanyWithUser(Company company, string username, string password);
        Company GetCompanyById(Guid id);
        bool IsCompanyExists(Guid id);
        Company GetCompanyByName(string name);
        IEnumerable<Company> GetCompaniesByName(string name);
        IEnumerable<Company> GetCompanies();

        OperationState UpdateCompany(Company company);
        OperationState UpdateCompanyLogo(string logoUrl, Guid companyId);
        Task<IEnumerable<CompanyCompound>> GetCompanyCompounds(Guid companyId);
    }

    public class CompanyService : BaseService, ICompanyService
    {
        private readonly ICompanyRepository companyRepository;
        private readonly ICompanyUserService companyUserService;
        private readonly ICompoundRepository _compoundRep;
        private readonly IServiceTypeRepository _servTypeRep;
        private readonly IIssueTypeRepository _issueTypeRep;

        public CompanyService(ICompanyRepository companyRepository, ICompanyUserService companyUserService,
                IUnitOfWork unitOfWork, IMapper mapper, ICompoundRepository compoundRep,
                IServiceTypeRepository servTypeRep, IIssueTypeRepository issueTypeRep)
                : base(unitOfWork, mapper)
        {
            this.companyRepository = companyRepository;
            this.companyUserService = companyUserService;
            _compoundRep = compoundRep;
            _servTypeRep = servTypeRep;
            _issueTypeRep = issueTypeRep;
        }

        public OperationState AddCompany(Company company)
        {
            company.CompanyId = Guid.NewGuid();
            company.CreationDate = DateTime.UtcNow;
            companyRepository.Add(company);
            int result = unitOfWork.Commit();

            return result > 0 ? OperationState.Created : OperationState.None;
        }

        public async Task<OperationState> AddCompanyWithUser(Company company, string username, string password)
        {
            var addCompanyState = AddCompany(company);
            if (addCompanyState == OperationState.Created)
            {
                var addUserState = await companyUserService.AddUser(new CompanyUser
                {
                    CompanyId = company.CompanyId,
                    IsActive = true,
                    IsDeleted = false,
                    Username = username,
                    Password = password,
                    NameAr = company.Name_Ar,
                    NameEn = company.Name_En,
                    Phone = company.Phone,
                    Email = company.Email,
                    UserType = (int)UserType.Company
                });
                return addUserState;
            }

            return addCompanyState;
        }

        public IEnumerable<Company> GetCompanies()
        {
            return companyRepository.GetMany(c => c.IsDeleted == false);
        }

        public IEnumerable<Company> GetCompaniesByName(string name)
        {
            return companyRepository.GetMany(c => (c.Name_Ar.Contains(name) || c.Name_En.Contains(name)) && c.IsDeleted == false);
        }

        public Company GetCompanyById(Guid id)
        {
            return companyRepository.Get(c => c.CompanyId == id && c.IsDeleted == false);
        }

        public Company GetCompanyByName(string name)
        {
            return companyRepository.Get(c => (c.Name_Ar.Contains(name) || c.Name_En.Contains(name)) && c.IsDeleted == false);
        }

        public bool IsCompanyExists(Guid id)
        {
            return companyRepository.Get(c => c.CompanyId == id && c.IsDeleted == false) != null;
        }

        public OperationState UpdateCompanyLogo(string logoUrl, Guid companyId)
        {
            var company = GetCompanyById(companyId);
            company.Logo = logoUrl;

            companyRepository.Update(company);

            return unitOfWork.Commit() > 0 ? OperationState.Updated : OperationState.None;
        }

        public OperationState UpdateCompany(Company company)
        {
            companyRepository.Update(company);

            return unitOfWork.Commit() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<IEnumerable<CompanyCompound>> GetCompanyCompounds(Guid companyId)
        {
            var compounds = await _compoundRep.Table.Where(z => z.CompanyId == companyId)
                .ToListAsync();
            var resultCompounds = new List<CompanyCompound>();
            foreach (var compound in compounds)
            {
                var newCompound = mapper.Map<CompanyCompound>(compound);
                var services = await _servTypeRep.Table.Include(x => x.CompoundServices)
                    .Where(x => x.IsFixed || x.CompoundServices.Any(c => c.CompoundId == compound.CompoundId)).ToListAsync();
                if (services.Any())
                    newCompound.Services = mapper.Map<List<CompoundServiceOutput>>(services);
                var issues = await _issueTypeRep.Table.Include(x => x.CompoundIssues)
                .Where(x => x.IsFixed || x.CompoundIssues.Any(c => c.CompoundId == compound.CompoundId)).ToListAsync();
                if (issues.Any())
                    newCompound.Issues = mapper.Map<List<CompoundIssueOutput>>(issues);
                resultCompounds.Add(newCompound);
            }
            return resultCompounds;
        }

    }
}
