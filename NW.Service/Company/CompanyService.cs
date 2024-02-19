using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Model;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Services
{ 
    public class CompanyService : BaseService, ICompanyService
    {
        IRepository<Company, int> CompanyRepository { get; set; }
        private ICompanyDomainRepository CompanyDomainRepository { get; set; } 
        private ICompanySettingRepository CompanySettingRepository { get; set; }
        IRepository<CompanyBackOfficeDomain, int> CompanyBackOfficeDomainRepository { get; set; }

        public CompanyService(ICompanyDomainRepository _companyDomainRepository, ICompanySettingRepository _companySettingRepository, 
        IRepository<Company, int> _companyRepository, IRepository<CompanyBackOfficeDomain, int> _companyBackOfficeDomainRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            CompanyDomainRepository = _companyDomainRepository;
            CompanySettingRepository = _companySettingRepository;
            CompanyRepository = _companyRepository;
            CompanyBackOfficeDomainRepository = _companyBackOfficeDomainRepository;
        }


        public virtual int CompanyId(string domain)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {

                int? companyId = CompanyDomainRepository.CompanyId(domain);
                return companyId.HasValue ? companyId.Value : 8;
            }
        }
        public virtual int BackOfficeCompanyId(string domain)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                CompanyBackOfficeDomain companyBackOfficeDomain = CompanyBackOfficeDomainRepository.GetAll().FirstOrDefault(cd => cd.Domain == domain);
                return companyBackOfficeDomain != null ? companyBackOfficeDomain.CompanyId : 1; // set default company 1 
            }
        }
        public virtual int BackOfficeVoltronCompanyId(string domain)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                CompanyBackOfficeDomain companyBackOfficeDomain = CompanyBackOfficeDomainRepository.GetAll().FirstOrDefault(cd => cd.Domain == domain);
                return companyBackOfficeDomain != null ? companyBackOfficeDomain.VoltronCompanyId : 6; // set default company 1 
            }
        }

        public virtual string GetValue(int companyId, string key, bool isProduction, bool useCache = true)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                return CompanySettingRepository.GetValue(companyId, key, isProduction);
            }
        }

        public void SetValue(int companyId, string key, string value, bool isProduction)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    CompanySettingRepository.SetValue(companyId, key, value, isProduction);
                    unitOfWork.Commit(transaction);
                }
            }
        }


        public Company Company(int id)
        {
            return CompanyRepository.Get(id);
        }
        public IList<Company> GetAllCompanies()
        {
            return CompanyRepository.GetAll().ToList();
        }
        public void InsertCompany(Company company)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    CompanyRepository.Insert(company);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateCompany(Company company)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                ITransaction transaction = unitOfWork.BeginTransaction(Session);
                CompanyRepository.Update(company);
                unitOfWork.Commit(transaction);
            }
        }
        public string GetCurrentDomain(int companyId)
        {
            return CompanyDomainRepository.GetAll().FirstOrDefault(cd => cd.CompanyId == companyId && cd.IsLive == true).Domain;
        }
    }
}