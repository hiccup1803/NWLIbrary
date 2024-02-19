using Newtonsoft.Json;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using NW.Core.Model.Marketing;
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
    public class MarketingBannerService : BaseService, IMarketingBannerService
    {
        IRepository<MarketingBanner, int> MarketingBannerRepository { get; set; }
        IRepository<MarketingBannerSize, int> MarketingBannerSizeRepository { get; set; }
        IRepository<MarketingBannerSizeItem, int> MarketingBannerSizeItemRepository { get; set; }
        IRepository<BannerAnnotation, int> BannerAnnotationRepository { get; set; }
        public MarketingBannerService(IRepository<MarketingBanner, int> _marketingBannerRepository, IRepository<MarketingBannerSize, int> _marketingBannerSizeRepository, IRepository<MarketingBannerSizeItem, int> _marketingBannerSizeItemRepository, IRepository<BannerAnnotation, int> _bannerAnnotationRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            MarketingBannerRepository = _marketingBannerRepository;
            MarketingBannerSizeRepository = _marketingBannerSizeRepository;
            MarketingBannerSizeItemRepository = _marketingBannerSizeItemRepository;
            BannerAnnotationRepository = _bannerAnnotationRepository;
        }

        #region  MarketingBanner
        public MarketingBanner MarketingBanner(int id)
        {
            return MarketingBannerRepository.Get(id);
        }
        public PagingModel<MarketingBanner> MarketingBanners(int pageIndex, int pageSize)
        {
            PagingModel<MarketingBanner> pagingModel = new PagingModel<MarketingBanner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBanner>()
                            .OrderBy(mt => mt.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public PagingModel<MarketingBanner> MarketingBanners(int pageIndex, int pageSize, int companyId)
        {
            PagingModel<MarketingBanner> pagingModel = new PagingModel<MarketingBanner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerRepository.GetAll().Where(mt => mt.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBanner>()
                            .Where(mt => mt.CompanyId == companyId)
                            .OrderBy(mt => mt.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public MarketingBanner InsertMarketingBanner(MarketingBanner marketingBanner)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBanner.CreateDate = DateTime.Now;
                    marketingBanner = MarketingBannerRepository.Insert(marketingBanner);
                    unitOfWork.Commit(transaction);
                    return marketingBanner;
                }
            }

        }
        public MarketingBanner UpdateMarketingBanner(MarketingBanner marketingBanner)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBanner = MarketingBannerRepository.Update(marketingBanner);
                    unitOfWork.Commit(transaction);
                    return marketingBanner;
                }
            }

        }
        #endregion
       
        #region MarketingBannerSize
        public MarketingBannerSize MarketingBannerSize(int id)
        {
            return MarketingBannerSizeRepository.Get(id);

        }
        public PagingModel<MarketingBannerSize> MarketingBannerSizes(int pageIndex, int pageSize)
        {
            PagingModel<MarketingBannerSize> pagingModel = new PagingModel<MarketingBannerSize>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBannerSize>()
                            .OrderBy(mt => mt.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public PagingModel<MarketingBannerSize> MarketingBannerSizesForMarketingBanner(int pageIndex, int pageSize, int marketingBannerId)
        {
            PagingModel<MarketingBannerSize> pagingModel = new PagingModel<MarketingBannerSize>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeRepository.GetAll().Where(m => m.MarketingBannerId == marketingBannerId).Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBannerSize>()
                            .Where(m => m.MarketingBannerId == marketingBannerId)
                            .OrderBy(m => m.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public PagingModel<MarketingBanner> MarketingBannersforAffiliate(int pageIndex, int pageSize)
        {
            PagingModel<MarketingBanner> pagingModel = new PagingModel<MarketingBanner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerRepository.GetAll().Where(m => m.StatusType == (int)StatusType.Active).Count();

                    pagingModel.ItemList = Session.QueryOver<MarketingBanner>()
                            .Where(m => m.StatusType == (int)StatusType.Active)
                            .OrderBy(m => m.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public PagingModel<MarketingBannerSize> MarketingBannerSizesforAffiliate(int pageIndex, int pageSize)
        {
            PagingModel<MarketingBannerSize> pagingModel = new PagingModel<MarketingBannerSize>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeRepository.GetAll().Where(m => m.MarketingBanner.StatusType == (int)StatusType.Active && m.StatusType == (int)StatusType.Active).Count();

                    pagingModel.ItemList = MarketingBannerSizeRepository.GetAll()
                            .Where(m => m.MarketingBanner.StatusType == (int)StatusType.Active && m.StatusType == (int)StatusType.Active)
                            .OrderByDescending(m => m.MarketingBanner.CreateDate).ThenByDescending(m => m.CreateDate)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();
                }
            }
            return pagingModel;
        }
        public PagingModel<MarketingBannerSize> MarketingBannerSizesforAffiliate(int pageIndex, int pageSize, int marketingBannerId)
        {
            PagingModel<MarketingBannerSize> pagingModel = new PagingModel<MarketingBannerSize>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeRepository.GetAll().Where(m => m.MarketingBannerId == marketingBannerId && m.StatusType == (int)StatusType.Active).Count();

                    pagingModel.ItemList = MarketingBannerSizeRepository.GetAll()
                            .Where(m => m.MarketingBannerId == marketingBannerId && m.StatusType == (int)StatusType.Active)
                            .OrderByDescending(m => m.MarketingBanner.CreateDate).ThenByDescending(m => m.CreateDate)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToList();
                }
            }
            return pagingModel;
        }
        public IList<MarketingBannerSize> MarketingBannerSizesForMarketingBanner(int marketingBannerId)
        {
            return MarketingBannerSizeRepository.GetAll().Where(mbs => mbs.MarketingBannerId == marketingBannerId).ToList();
        }
        public int MarketingBannerSizeCountForMarketingBanner(int marketingBannerId)
        {
            return MarketingBannerSizeRepository.GetAll().Count(mbs => mbs.MarketingBannerId == marketingBannerId);
        }
        public int ActiveMarketingBannerSizeCountForMarketingBanner(int marketingBannerId)
        {
            return MarketingBannerSizeRepository.GetAll().Count(mbs => mbs.MarketingBannerId == marketingBannerId && mbs.StatusType == (int)StatusType.Active);
        }
        public MarketingBannerSize InsertMarketingBannerSize(MarketingBannerSize marketingBannerSize)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBannerSize.CreateDate = DateTime.Now;
                    marketingBannerSize = MarketingBannerSizeRepository.Insert(marketingBannerSize);

                    unitOfWork.Commit(transaction);

                    return marketingBannerSize;
                }
            }

        }
        public MarketingBannerSize UpdateMarketingBannerSize(MarketingBannerSize marketingBannerSize)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBannerSize = MarketingBannerSizeRepository.Update(marketingBannerSize);

                    unitOfWork.Commit(transaction);
                    return marketingBannerSize;
                }
            }

        }
        #endregion

        #region MarketingBannerSizeItem
        public MarketingBannerSizeItem MarketingBannerSizeItem(int id)
        {
            return MarketingBannerSizeItemRepository.Get(id);
        }
        public PagingModel<MarketingBannerSizeItem> MarketingBannerSizeItems(int pageIndex, int pageSize)
        {
            PagingModel<MarketingBannerSizeItem> pagingModel = new PagingModel<MarketingBannerSizeItem>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeItemRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBannerSizeItem>()
                            .OrderBy(m => m.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public PagingModel<MarketingBannerSizeItem> MarketingBannerSizeItemsForMarketingBannerSize(int pageIndex, int pageSize, int marketingBannerSizeId)
        {

            PagingModel<MarketingBannerSizeItem> pagingModel = new PagingModel<MarketingBannerSizeItem>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MarketingBannerSizeItemRepository.GetAll().Where(b => b.MarketingBannerSizeId == marketingBannerSizeId).Count();
                    pagingModel.ItemList = Session.QueryOver<MarketingBannerSizeItem>()
                            .Where(b => b.MarketingBannerSizeId == marketingBannerSizeId)
                            .OrderBy(m => m.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public IList<MarketingBannerSizeItem> MarketingBannerSizeItemsForMarketingBannerSize(int marketingBannerSizeId)
        {
            return MarketingBannerSizeItemRepository.GetAll().Where(b => b.MarketingBannerSizeId == marketingBannerSizeId).OrderBy(b => b.DisplayOrder).ToList();
        }
        public MarketingBannerSizeItem InsertMarketingBannerSizeItem(MarketingBannerSizeItem marketingBannerSizeItem)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBannerSizeItem.CreateDate = DateTime.Now;
                    marketingBannerSizeItem = MarketingBannerSizeItemRepository.Insert(marketingBannerSizeItem);

                    unitOfWork.Commit(transaction);

                    return marketingBannerSizeItem;
                }
            }

        }
        public MarketingBannerSizeItem UpdateMarketingBannerSizeItem(MarketingBannerSizeItem marketingBannerSizeItem)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    marketingBannerSizeItem = MarketingBannerSizeItemRepository.Update(marketingBannerSizeItem);
                    unitOfWork.Commit(transaction);

                    return marketingBannerSizeItem;
                }
            }

        }
        public MarketingBannerSizeItem UpdateMarketingBannerSizeItemOrder(int marketingBannerSizeItemId, int order)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MarketingBannerSizeItem marketingBannerSizeItem = MarketingBannerSizeItemRepository.Get(marketingBannerSizeItemId);
                    if (marketingBannerSizeItem.DisplayOrder != order)
                    {
                        marketingBannerSizeItem.DisplayOrder = order;
                        marketingBannerSizeItem = MarketingBannerSizeItemRepository.Update(marketingBannerSizeItem);
                    }
                    unitOfWork.Commit(transaction);

                    return marketingBannerSizeItem;
                }
            }

        }
        #endregion

        #region annotation url
        public void InsertOrUpdateBannerAnnotationForMarketingBannerSize(int marketingBannerSizeId)
        {
            MarketingBannerSize marketingBannerSize = MarketingBannerSizeRepository.Get(marketingBannerSizeId);
            InsertOrUpdateBannerAnnotationForMarketingBannerSize(marketingBannerSize);
        }
        public void InsertOrUpdateBannerAnnotationForMarketingBannerSize(MarketingBannerSize marketingBannerSize)
        {

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    string url = PrepareAnnotationUrlForMarketingBannerSize(marketingBannerSize);
                    if (marketingBannerSize.BannerAnnotation != null)
                    {
                        BannerAnnotation bannerAnnotation = BannerAnnotationRepository.Get(marketingBannerSize.BannerAnnotation.Id);
                        bannerAnnotation.UrlToRedirect = url;
                        BannerAnnotationRepository.Update(bannerAnnotation);

                    }
                    else
                    {
                        BannerAnnotation bannerAnnotation = new BannerAnnotation();
                        bannerAnnotation.CreateDate = DateTime.Now;
                        bannerAnnotation.MarketingBannerSizeId = marketingBannerSize.Id;
                        bannerAnnotation.MarketingBannerSize = marketingBannerSize;
                        bannerAnnotation.UrlAlias = String.Format("banner/b{0}s{1}", marketingBannerSize.MarketingBannerId, marketingBannerSize.Id);
                        bannerAnnotation.UrlToRedirect = url;

                        BannerAnnotationRepository.Insert(bannerAnnotation);

                    }
                    unitOfWork.Commit(transaction);
                }
            }

        }
        public string PrepareAnnotationUrlForMarketingBannerSize(int marketingBannerSizeId)
        {
            MarketingBannerSize marketingBannerSize = MarketingBannerSizeRepository.Get(marketingBannerSizeId);
            return PrepareAnnotationUrlForMarketingBannerSize(marketingBannerSize);
        }
        public string PrepareAnnotationUrlForMarketingBannerSize(MarketingBannerSize marketingBannerSize)
        {
            MarketingBannerJsonModel jsonObject = new MarketingBannerJsonModel();
            string htmlFileName = string.Empty;
            jsonObject.Width = marketingBannerSize.Width;
            jsonObject.Height = marketingBannerSize.Height;

            if (string.IsNullOrEmpty(marketingBannerSize.IframeUrl))
            {
                htmlFileName = "banner";
                if(marketingBannerSize.MarketingBannerSizeItems != null && marketingBannerSize.MarketingBannerSizeItems.Count > 0)
                {
                    List<MarketingBannerSizeItem> marketingBannerSizeItems = marketingBannerSize.MarketingBannerSizeItems.Where(b => b.StatusType == (int)StatusType.Active).OrderBy(b => b.DisplayOrder).ToList();
                    jsonObject.Items = new List<MarketingBannerItemJsonModel>();
                    foreach (MarketingBannerSizeItem item in marketingBannerSizeItems)
                    {
                        jsonObject.Items.Add(new MarketingBannerItemJsonModel()
                        {
                            Url = item.Url,
                            ImagePath = item.ImagePath
                        });
                    }
                }

            }
            else
            {
                htmlFileName = "iframe";
                jsonObject.IframeUrl = marketingBannerSize.IframeUrl;
                jsonObject.Url = marketingBannerSize.Url;
            }

            return String.Format("https://{0}/assets/{1}/content/staticpages/{2}.html?content={3}", ConfigurationManager.AppSettings["CDNURL"], ConfigurationManager.AppSettings["CompanyId"], htmlFileName, RestSharp.Extensions.MonoHttp.HttpUtility.UrlEncode(JsonConvert.SerializeObject(jsonObject)));

        }
        #endregion
    }
}
