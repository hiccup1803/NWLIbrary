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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Services
{
    public class FAQService : BaseService, IFAQService
    {
        private IFAQRepository FAQRepository { get; set; }

        public FAQService(IFAQRepository _faqRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            FAQRepository = _faqRepository;
        }

        public FAQ GetFAQ(int id)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return FAQRepository.Get(id);
            }
        }

        public FAQ InsertFAQ(FAQ faq)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    faq = FAQRepository.Insert(faq);
                    unitOfWork.Commit(transaction);
                    return faq;
                }
            }
        }

        public FAQ GetFAQ(string pageName, int companyId, string languageCode)
        {
            throw new NotImplementedException();
        }

        public FAQ GetFAQ(int pageId, int companyId, string languageCode)
        {
            throw new NotImplementedException();
        }

        public FAQ UpdateFAQ(FAQ faq)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    faq = FAQRepository.Update(faq);
                    unitOfWork.Commit(transaction);
                    return faq;
                }
            }
        }

        public PagingModel<FAQ> FAQs(int pageIndex, int pageSize)
        {
            PagingModel<FAQ> pagingModel = new PagingModel<FAQ>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = FAQRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<FAQ>()
                        .Where(cp => cp.Status != 0)
                            .OrderBy(cp => cp.Id).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }

        public PagingModel<FAQ> FAQs(int pageIndex, int pageSize, int companyId)
        {
            PagingModel<FAQ> pagingModel = new PagingModel<FAQ>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = FAQRepository.GetAll().Where(cp => cp.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<FAQ>()
                            .Where(cp => cp.CompanyId == companyId && cp.Status != 0)
                            .OrderBy(cp => cp.Id).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
    }


}