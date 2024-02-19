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

namespace NW.Service
{
    public class AffiliateService : BaseService, IAffiliateService
    {
        IRepository<AffiliateType, int> AffiliateTypeRepository { get; set; }

        public AffiliateService(IRepository<AffiliateType, int> _affiliateTypeRepository, IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            AffiliateTypeRepository = _affiliateTypeRepository;
        }

        public IList<AffiliateType> GetAllAffiliateTypes()
        {
            return AffiliateTypeRepository.GetAll().ToList();
        }
    }
}
