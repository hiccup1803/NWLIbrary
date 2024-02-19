using NW.Core.Entities;
using NW.Core.Entities.Marketing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMarketingBannerService
    {
        #region  MarketingBanner
        MarketingBanner MarketingBanner(int id);
        PagingModel<MarketingBanner> MarketingBanners(int pageIndex, int pageSize);
        PagingModel<MarketingBanner> MarketingBanners(int pageIndex, int pageSize, int companyId);
        MarketingBanner InsertMarketingBanner(MarketingBanner marketingBanner);
        MarketingBanner UpdateMarketingBanner(MarketingBanner marketingBanner);
        #endregion

        #region MarketingBannerSize
        MarketingBannerSize MarketingBannerSize(int id);
        PagingModel<MarketingBannerSize> MarketingBannerSizes(int pageIndex, int pageSize);
        PagingModel<MarketingBanner> MarketingBannersforAffiliate(int pageIndex, int pageSize);
        PagingModel<MarketingBannerSize> MarketingBannerSizesforAffiliate(int pageIndex, int pageSize);
        PagingModel<MarketingBannerSize> MarketingBannerSizesforAffiliate(int pageIndex, int pageSize, int marketingBannerId);
        PagingModel<MarketingBannerSize> MarketingBannerSizesForMarketingBanner(int pageIndex, int pageSize, int marketingBannerId);
        int MarketingBannerSizeCountForMarketingBanner(int marketingBannerId);
        int ActiveMarketingBannerSizeCountForMarketingBanner(int marketingBannerId);
        IList<MarketingBannerSize> MarketingBannerSizesForMarketingBanner(int marketingBannerId);
        MarketingBannerSize InsertMarketingBannerSize(MarketingBannerSize marketingBannerSize);
        MarketingBannerSize UpdateMarketingBannerSize(MarketingBannerSize marketingBannerSize);
        #endregion

        #region MarketingBannerSizeItem
        MarketingBannerSizeItem MarketingBannerSizeItem(int id);
        PagingModel<MarketingBannerSizeItem> MarketingBannerSizeItems(int pageIndex, int pageSize);
        PagingModel<MarketingBannerSizeItem> MarketingBannerSizeItemsForMarketingBannerSize(int pageIndex, int pageSize, int marketingBannerSizeId);
        IList<MarketingBannerSizeItem> MarketingBannerSizeItemsForMarketingBannerSize(int marketingBannerSizeId);
        MarketingBannerSizeItem InsertMarketingBannerSizeItem(MarketingBannerSizeItem marketingBannerSizeItem);
        MarketingBannerSizeItem UpdateMarketingBannerSizeItem(MarketingBannerSizeItem marketingBannerSizeItem);
        MarketingBannerSizeItem UpdateMarketingBannerSizeItemOrder(int marketingBannerSizeItemId, int order);
        #endregion

        #region annotation url
        void InsertOrUpdateBannerAnnotationForMarketingBannerSize(int marketingBannerSizeId);
        void InsertOrUpdateBannerAnnotationForMarketingBannerSize(MarketingBannerSize marketingBannerSize);
        string PrepareAnnotationUrlForMarketingBannerSize(int marketingBannerSizeId);
        string PrepareAnnotationUrlForMarketingBannerSize(MarketingBannerSize marketingBannerSize);
        #endregion
    }
}
