using NHibernate;
using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using NW.Core.Repositories;
using NW.Core.Services.Marketing;
using NW.Core.Work;
using NW.Helper.Optimove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Marketing
{
    public class MarketingService : BaseService, IMarketingService
    {
        IRepository<OptimoveTemplate, int> OptimoveTemplateRespository { get; set; }
        public MarketingService(IRepository<OptimoveTemplate, int> _optimoveTemplateRespository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            OptimoveTemplateRespository = _optimoveTemplateRespository;
        }
        public IList<Core.Entities.Marketing.OptimoveTemplate> GetOptimoveTemplateList(int? templateType)
        {
            IQueryable<OptimoveTemplate> query = OptimoveTemplateRespository.GetAll().Where(ot => ot.StatusType == (int)StatusType.Active);
            if (templateType.HasValue)
                query = query.Where(ot => ot.TemplateType == templateType.Value);


            return query.OrderByDescending(ot => ot.CreateDate).ToList();
        }
        private int GetOptimoveChannelId(TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.Email:
                    return OptimoveHelper.EMAIL_CHANNEL;
                case TemplateType.SMS:
                    return OptimoveHelper.SMS_CHANNEL;
                default:
                    return -1;
            }
        }
        public void InsertOptimoveTemplate(Core.Enum.TemplateType templateType, Core.Enum.StatusType statusType, string name, string content)
        {
            using (ITransaction transaction = UnitOfWork.Current.BeginTransaction(Session))
            {
                OptimoveTemplate optimoveTemplate = OptimoveTemplateRespository.Insert(new OptimoveTemplate() { Name = name, TemplateType = (int)templateType, CreateDate = DateTime.UtcNow, StatusType = (int)statusType, Content = content });
                bool result = OptimoveHelper.AddTemplate(GetOptimoveChannelId(templateType), optimoveTemplate.Id, optimoveTemplate.Name);
                if (result)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        public void DeleteOptimoveTemplate(int id)
        {
            using (ITransaction transaction = UnitOfWork.Current.BeginTransaction(Session))
            {
                OptimoveTemplate optimoveTemplate = OptimoveTemplateRespository.Get(id);
                optimoveTemplate.StatusType = (int)StatusType.Deleted;
                optimoveTemplate.UpdateDate = DateTime.UtcNow;
                OptimoveTemplateRespository.Update(optimoveTemplate);

                bool result = OptimoveHelper.DeleteTemplate(GetOptimoveChannelId((TemplateType)optimoveTemplate.TemplateType), optimoveTemplate.Id);
                if (result)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }
    }
}
