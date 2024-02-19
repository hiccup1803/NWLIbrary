using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.ContentManagement
{
    public class BannerService : BaseService, IBannerService
    {
        private DateTime trNow = DateTime.UtcNow.AddHours(3); //incredible turkish technology
        IRepository<Banner, int> BannerRepository { get; set; }
        IMemberRepository MemberRepository { get; set; }
        IRepository<BannerMemberReaction, int> BannerMemberReactionRepository { get; set; }
        IMemberSegmentService MemberSegmentService { get; set; }

        public BannerService(IRepository<Banner, int> _bannerRepository, IRepository<BannerMemberReaction, int> _bannerMemberReactionRepository, IMemberRepository _memberRepository, IMemberSegmentService _memberSegmentService, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            BannerRepository = _bannerRepository;
            BannerMemberReactionRepository = _bannerMemberReactionRepository;
            MemberSegmentService = _memberSegmentService;
            MemberRepository = _memberRepository;
        }

        public Banner Banner(int id)
        {
            return BannerRepository.Get(id);
        }

        public PagingModel<Banner> GetAllBanners(int pageIndex, int pageSize, int companyId)
        {
            PagingModel<Banner> pagingModel = new PagingModel<Banner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = BannerRepository.GetAll().Where(b => b.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<Banner>()
                            .Where(b => b.CompanyId == companyId)
                            .OrderBy(b => b.DisplayOrder).Desc
                            //.ThenBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public IList<Banner> GetAllBanners()
        {
            return BannerRepository.GetAll().OrderByDescending(b => b.DisplayOrder).ToList();
        }
        public PagingModel<Banner> BannersForBannerPlace(int pageIndex, int pageSize, int bannerPlaceId, int companyId)
        {
            PagingModel<Banner> pagingModel = new PagingModel<Banner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<Banner>()
                            .Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId)
                            .OrderBy(t => t.Active).Desc
                            .ThenBy(b => b.DisplayOrder).Desc
                            //.ThenBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public PagingModel<Banner> BannersForBannerPlace(int pageIndex, int pageSize, int bannerPlaceId, int companyId, int languageId)
        {
            PagingModel<Banner> pagingModel = new PagingModel<Banner>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.LanguageId == languageId && (b.StatusType == 0 || b.StatusType == 1)).Count();
                    pagingModel.ItemList = Session.QueryOver<Banner>()
                            .Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.LanguageId == languageId && (b.StatusType == 0 || b.StatusType == 1))
                            .OrderBy(t => t.Active).Desc
                            .ThenBy(b => b.DisplayOrder).Desc
                            //.ThenBy(t => t.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public IList<Banner> ActiveBannersForBannerPlace(int bannerPlaceId, int companyId)
        {
            return BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.Active == true).OrderByDescending(b => b.DisplayOrder).ToList();
        }
        public IList<Banner> ActiveBannersForBannerPlace(int bannerPlaceId, int companyId, int languageId, bool isVip)
        {
            return BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.LanguageId == languageId && b.IsVip == isVip && b.Active == true && b.StatusType == 1).OrderByDescending(b => b.DisplayOrder).ToList();
        }
        public int ActiveBannerCountForBannerPlace(int bannerPlaceId, int companyId)
        {
            return BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.Active == true).Count();
        }
        public int PassiveBannerCountForBannerPlace(int bannerPlaceId, int companyId)
        {
            return BannerRepository.GetAll().Where(b => b.BannerPlace == bannerPlaceId && b.CompanyId == companyId && b.Active == false).Count();
        }
        public Banner InsertBanner(Banner banner)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    banner.CreateDate = DateTime.Now;
                    banner = BannerRepository.Insert(banner);
                    unitOfWork.Commit(transaction);
                    return banner;
                }
            }
        }
        public Banner UpdateBanner(Banner banner)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    banner = BannerRepository.Update(banner);
                    unitOfWork.Commit(transaction);
                    return banner;
                }
            }
        }


        public void UpdateBannerDisplayOrder(int bannerId, int displayOrder)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    Banner banner = BannerRepository.Get(bannerId);
                    if (banner.DisplayOrder != displayOrder)
                    {
                        banner.DisplayOrder = displayOrder;
                        BannerRepository.Update(banner);
                        unitOfWork.Commit(transaction);
                    }
                }
            }

        }


        public int GetReactionCountByBannerIdReactionId(int bannerId, int reactionType)
        {
            return BannerMemberReactionRepository.GetAll().Count(bmr => bmr.CMS_BannerId == bannerId && bmr.ReactionType == reactionType);
        }

        #region Web
        public IList<Banner> BannerForBannerPlace(int bannerPlaceId, int companyId, int languageId, bool isVip, int? memberId)
        {
            List<Banner> activeBanners = ActiveBannersForBannerPlace(bannerPlaceId, companyId, languageId, isVip).ToList();
            List<Banner> bannerList = new List<Banner>();
            Banner banner = new Banner();
            foreach (Banner b in activeBanners)
            {
                if (b.BannerType == (int)BannerType.WeeklyRepetitive)
                {
                    //if (!b.BannerDay.Split(',').Any(bd => Convert.ToInt32(bd) == (int)DateTime.UtcNow.DayOfWeek))
                    if (b.BannerDay != (int)DateTime.UtcNow.DayOfWeek)
                        continue;
                }
                else if (b.BannerType == (int)BannerType.WeeklyRepetitiveTimeInterval)
                {
                    //if (!b.BannerDay.Split(',').Any(bd => Convert.ToInt32(bd) == (int)DateTime.UtcNow.DayOfWeek))
                    if (b.BannerDay != (int)DateTime.UtcNow.DayOfWeek)
                        continue;
                    if (DateTime.UtcNow.TimeOfDay > b.EndTime || DateTime.UtcNow.TimeOfDay < b.StartTime)
                        continue;
                }
                else if (b.BannerType == (int)BannerType.RepetitiveTimeInterval)
                {
                    if (DateTime.UtcNow.TimeOfDay > b.EndTime || DateTime.UtcNow.TimeOfDay < b.StartTime)
                        continue;
                }
                else if (b.BannerType == (int)BannerType.MonthlyRepetitive)
                {
                    if (b.BannerDay != (int)DateTime.UtcNow.Day)
                        continue;
                }
                else if (b.BannerType == (int)BannerType.TimeInterval)
                {
                    if (DateTime.UtcNow > b.EndDate || DateTime.UtcNow < b.StartDate)
                        continue;
                }
                if (b.MemberSegments.Count > 0)
                {
                    if (memberId.HasValue)
                    {
                        if (!MemberSegmentService.IsFilterHasMember(b.MemberSegments.Select(ms => ms.Id).ToArray(), memberId.Value))
                            continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (b.BannerUsernameFilterType != (int)BannerUsernameFilterType.Public)
                {
                    if (memberId.HasValue)
                    {
                        Core.Entities.Member member = MemberRepository.Get(memberId.Value);

                        if (member == null)
                        {
                            continue;
                        }
                        if (b.BannerUsernameFilterType == (int)BannerUsernameFilterType.RestrictedForUsers && b.UsernameList.Split(',').Contains(member.Username))
                        {
                            continue;
                        }
                        if (b.BannerUsernameFilterType == (int)BannerUsernameFilterType.AvailableForUsers && !b.UsernameList.Split(',').Contains(member.Username))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                bannerList.Add(b);
            }
            return bannerList;
        }


        public int? GetReactionTypeByMemberIdBannerId(int memberId, int bannerId)
        {
            BannerMemberReaction bannerMemberReaction = BannerMemberReactionRepository.GetAll().FirstOrDefault(b => b.MemberId == memberId && b.CMS_BannerId == bannerId);
            return bannerMemberReaction != null ? bannerMemberReaction.ReactionType : new Nullable<int>();
        }

        public void UpdateBannerReaction(int reactionType, int memberId, int bannerId)
        {

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    BannerMemberReaction bannerMemberReaction = BannerMemberReactionRepository.GetAll().FirstOrDefault(bmr => bmr.MemberId == memberId && bmr.CMS_BannerId == bannerId);
                    if (bannerMemberReaction == null)
                    {
                        BannerMemberReactionRepository.Insert(new BannerMemberReaction() { CMS_BannerId = bannerId, ReactionType = reactionType, MemberId = memberId, CreateDate = DateTime.UtcNow });
                    }
                    else
                    {
                        bannerMemberReaction.ReactionType = reactionType;
                        BannerMemberReactionRepository.Update(bannerMemberReaction);
                    }
                    unitOfWork.Commit(transaction);
                }
            }
        }
        #endregion
    }
}