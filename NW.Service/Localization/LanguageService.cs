using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Localization
{
    public class LanguageService : BaseService, ILanguageService
    {
        IRepository<Language, int> LanguageRepository { get; set; }
        IRepository<Resource, int> ResourceRepository { get; set; }
        public LanguageService(IRepository<Language, int> _languageRepository, IRepository<Resource, int> _resourceRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            LanguageRepository = _languageRepository;
            ResourceRepository = _resourceRepository;
        }
        public IList<Core.Entities.Language> Languages()
        {
            return LanguageRepository.GetAll().ToList();
        }

        public Language Language(int id)
        {
            return LanguageRepository.Get(id);
        }
        public Language Language(string lang)
        {
            return LanguageRepository.GetAll().FirstOrDefault(l => l.IVRCode == lang);
        }
        public Resource Resource(int id)
        {
            return ResourceRepository.Get(id);

        }
        public Resource Resource(Resource resource)
        {
            return ResourceRepository.GetAll().FirstOrDefault(r => r.ClassName == resource.ClassName && r.Culture == resource.Culture && r.ResourceName == resource.ResourceName);
        }
        public Resource Resource(string className, string culture, string resourceName)
        {
            return ResourceRepository.GetAll().FirstOrDefault(r => r.ClassName == className && r.Culture == culture && r.ResourceName == resourceName);
        }
        public Resource Resource(string className, int languageId, string resourceName)
        {
            return ResourceRepository.GetAll().FirstOrDefault(r => r.ClassName == className && r.LanguageId == languageId && r.ResourceName == resourceName);
        }
        public void InsertResource(Resource resource)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    ResourceRepository.Insert(resource);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateResource(Resource resource)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    ResourceRepository.Update(resource);
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public bool ResourceNameExists(Resource resource)
        {
            return ResourceNameExists(resource.ClassName, resource.LanguageId, resource.ResourceName);
        }
        public bool ResourceNameExists(string className, int languageId, string resourceName)
        {
            return ResourceRepository.GetAll().Any(r => r.ClassName == className && r.LanguageId == languageId && r.ResourceName == resourceName);
        }
    }
}
