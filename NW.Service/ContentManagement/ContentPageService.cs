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
    public class ContentPageService : BaseService, IContentPageService
    {
        private IContentPageRepository ContentPageRepository { get; set; }

        public ContentPageService(IContentPageRepository _contentPageRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            ContentPageRepository = _contentPageRepository;
        }
        public ContentPage ContentPage(int id)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ContentPageRepository.Get(id);
            }

        }

        public ContentPage InsertContentPage(ContentPage contentPage)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    contentPage = ContentPageRepository.Insert(contentPage);
                    unitOfWork.Commit(transaction);
                    return contentPage;
                }
            }
        }
        public ContentPage UpdateContentPage(ContentPage contentPage)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    contentPage = ContentPageRepository.Update(contentPage);
                    unitOfWork.Commit(transaction);
                    return contentPage;
                }
            }
        }

        public ContentPage GetContent(string pageName, int companyId, string languageCode)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                return ContentPageRepository.GetContent(pageName, companyId, languageCode);
            }
        }

        public ContentPage GetContent(int pageId, int companyId, string languageCode)
        {
            return ContentPageRepository.GetContent(pageId, companyId, languageCode);
        }
        public PagingModel<ContentPage> ContentPages(int pageIndex, int pageSize)
        {

            PagingModel<ContentPage> pagingModel = new PagingModel<ContentPage>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = ContentPageRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<ContentPage>()
                            .OrderBy(cp => cp.Id).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }

        public PagingModel<ContentPage> ContentPages(int pageIndex, int pageSize, int companyId)
        {

            PagingModel<ContentPage> pagingModel = new PagingModel<ContentPage>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = ContentPageRepository.GetAll().Where(cp => cp.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<ContentPage>()
                            .Where(cp => cp.CompanyId == companyId)
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