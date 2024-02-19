using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Services
{
    public interface IPromotionService
    {
        List<Promotion> GetAllPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false);

        List<Promotion> GetNonTRPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false);

        List<Promotion> GetVIPPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false);
        List<Promotion> GetAllPromotions(int memberId, int companyId, bool onlyActive = false);

        List<Promotion> GetVIPPromotions(int memberId, int companyId, bool onlyActive = false);
        List<Promotion> GetPromotions(int memberId, int companyId, bool isVip, bool onlyActive = false);

        bool Apply(string domain, int memberId, bool isProduction, int promotionId);
        bool Forfeit(string domain, int memberId, bool isProduction, int promotionId);
        string GetPromoTermsById(string domain, int memberId, bool isProduction, int promoId);
        Promotion Get(int promotionId);
        Promotion Get(string name);

        PagingModel<Promotion> GetPromotions(int companyId, int pageIndex, int pageSize);
        Promotion InsertPromotion(Promotion promotion);
        Promotion UpdatePromotion(Promotion promotion);

        void UpdatePromotionDisplayOrder(int promotionId, int displayOrder);
        bool CheckPromotionForUsername(int promotionId, string username);
        bool CheckPromotionForUsername(Promotion promotion, string username);
        IList<Promotion> GetPromotionsForMember(Member member);
    }
}
