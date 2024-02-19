using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IBannerService
    {
        Banner Banner(int id);
        PagingModel<Banner> GetAllBanners(int pageIndex, int pageSize, int companyId);
        IList<Banner> GetAllBanners();
        PagingModel<Banner> BannersForBannerPlace(int pageIndex, int pageSize, int bannerPlaceId, int companyId);
        PagingModel<Banner> BannersForBannerPlace(int pageIndex, int pageSize, int bannerPlaceId, int companyId, int languageId);
        IList<Banner> ActiveBannersForBannerPlace(int bannerPlaceId, int companyId);
        IList<Banner> ActiveBannersForBannerPlace(int bannerPlaceId, int companyId, int languageId, bool isVip);
        int ActiveBannerCountForBannerPlace(int bannerPlaceId, int companyId);
        int PassiveBannerCountForBannerPlace(int bannerPlaceId, int companyId);
        Banner InsertBanner(Banner banner);
        Banner UpdateBanner(Banner banner);
        void UpdateBannerDisplayOrder(int bannerId, int displayOrder);
        int GetReactionCountByBannerIdReactionId(int bannerId, int reactionType);
        #region Web
        IList<Banner> BannerForBannerPlace(int bannerPlaceId, int companyId, int languageId, bool isVip, int? memberId);
        int? GetReactionTypeByMemberIdBannerId(int memberId, int bannerId);
        void UpdateBannerReaction(int reactionType, int memberId, int bannerId);
        #endregion
    }
}
