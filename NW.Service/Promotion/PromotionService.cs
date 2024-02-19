using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Data.NHibernate.Repositories;
using NW.Data.NHibernate.Work;
using NW.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NW.Core.Enum;

namespace NW.Services
{
    public class PromotionService : BaseService, IPromotionService
    {
        private DateTime trNow = DateTime.UtcNow.AddHours(3);
        public IPromotionRepository PromotionRepository { get; set; }
        public IRepository<CustomStuff,int> CustomStuffRepository { get; set; }

        public PromotionService(
            IPromotionRepository _promotionRepository,
            IRepository<CustomStuff,int> _customStuffRepository,
            IUnitOfWork _unitOfWork,
            ISession _session)
            : base(_unitOfWork, _session)
        {
            PromotionRepository = _promotionRepository;
            CustomStuffRepository = _customStuffRepository;
        }

        public string GetPromoTermsById(string domain, int memberId, bool isProduction, int promoId)
        {
            var p= PromotionRepository.Get(promoId);
            if (p!=null)
            {
                if (p.Terms.StartsWith("http://") || p.Terms.StartsWith("https://"))
                {
                    var textFromFile = (new WebClient()).DownloadString(p.Terms);
                    return textFromFile;
                }
                return p.Terms;
            }

            return "";
        }

        public List<Promotion> GetAllPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.Active && w.CountryId == 1 && w.IsVipPromo == false && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }

        public List<Promotion> GetNonTRPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.CountryId > 1 && w.Active && w.IsVipPromo == false && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }

        public List<Promotion> GetVIPPromotions(string domain, int memberId, bool isProduction, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.Active && w.CountryId == 1 && w.IsVipPromo && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.IsVipPromo && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }



        public List<Promotion> GetAllPromotions(int memberId, int companyId, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.Active && w.CountryId == companyId && w.IsVipPromo == false && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }
        public List<Promotion> GetVIPPromotions(int memberId, int companyId, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.Active && w.CountryId == companyId && w.IsVipPromo && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.IsVipPromo && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }

        public List<Promotion> GetPromotions(int memberId, int companyId, bool isVip, bool onlyActive = false)
        {
            if (onlyActive)
            {
                return PromotionRepository.GetAll().Where(w => w.Active && w.CountryId == companyId && w.IsVipPromo == isVip && w.CountryId == companyId  && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
            }
            return PromotionRepository.GetAll().Where(w => w.IsVipPromo == isVip && w.CountryId == companyId && w.ExpireDate > DateTime.UtcNow.AddDays(-1) && w.StartDate <= DateTime.UtcNow).OrderBy(w => w.OrderNumber).ToList();
        }

        public Promotion Get(int promotionId)
        {
            return PromotionRepository.Get(promotionId);
        }
        public Promotion Get(string name)
        {
            return PromotionRepository.GetAll().SingleOrDefault(w=>w.Name==name && w.Active == true);
        }


        public bool Apply(string domain, int memberId, bool isProduction, int promotionId)
        {
            throw new NotImplementedException();
        }

        public bool Forfeit(string domain, int memberId, bool isProduction, int promotionId)
        {
            throw new NotImplementedException();
        }


        public PagingModel<Promotion> GetPromotions(int companyId, int pageIndex, int pageSize)
        {
            PagingModel<Promotion> pagingModel = new PagingModel<Promotion>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = PromotionRepository.GetAll().Where(p => p.CountryId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<Promotion>()
                            .Where(p => p.CountryId == companyId)
                            .OrderBy(p => p.Active).Desc
                            .ThenBy(p => p.ExpireDate).Desc
                            .ThenBy(p => p.OrderNumber).Asc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;

        }
        public Promotion InsertPromotion(Promotion promotion)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    promotion.CreateDate = DateTime.Now;
                    promotion = PromotionRepository.Insert(promotion);
                    unitOfWork.Commit(transaction);
                    return promotion;
                }
            }
        }
        public Promotion UpdatePromotion(Promotion promotion)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    promotion = PromotionRepository.Update(promotion);
                    unitOfWork.Commit(transaction);
                    return promotion;
                }
            }
        }

        public void UpdatePromotionDisplayOrder(int promotionId, int displayOrder)
        {

            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    Promotion promotion = PromotionRepository.Get(promotionId);
                    if (promotion.OrderNumber != displayOrder)
                    {
                        promotion.OrderNumber = displayOrder;
                        PromotionRepository.Update(promotion);
                        unitOfWork.Commit(transaction);
                    }
                }
            }
        }

        public bool CheckPromotionForUsername(int promotionId, string username)
        {
            int customDataId = 2;
            if(promotionId == 178)
            {
                customDataId = 3;
            }
            if (promotionId == 180)
            {
                customDataId = 4;
            }
            JObject jObject = (JObject)JsonConvert.DeserializeObject(CustomStuffRepository.Get(customDataId).Data);
            if ((int)jObject["promotionId"] == promotionId && ((JArray)jObject["usernames"]).Any(u => string.Equals(u.ToString().Trim(), username, StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }
            return false;
        }

        public bool CheckPromotionForUsername(Promotion promotion, string username)
        {
            username = username.Trim();
            switch (promotion.PromotionType)
            {
                case (int)PromotionType.RestrictedForUsers:
                    return !promotion.UsernameList.Split(',').Contains(username) || string.IsNullOrEmpty(username);
                case (int)PromotionType.AvailableForUsers:
                    return promotion.UsernameList.Split(',').Contains(username) && !string.IsNullOrEmpty(username);
                case (int)PromotionType.Public:
                    return true;
                default:
                    return true;
            }
        }
       
        public IList<Promotion> GetPromotionsForMember(Member member)
        {
            //countryid
            IQueryable<Promotion> promotions = PromotionRepository.GetAll().OrderByDescending(p => p.Active).ThenByDescending(p => p.ExpireDate).ThenBy(t => t.OrderNumber);//.Where(p => p.Active == true && p.ExpireDate > DateTime.Now && p.StartDate <= DateTime.Now);
            promotions = promotions.Where(p => p.IsVipPromo == member.Level.Name.Contains("VIP"));
            return promotions.ToList().Where(p => CheckPromotionForUsername(p, member.Username) == true).ToList();
            //return promotions.ToList();

        }

        /*public List<Promotion> CheckPromotionForUsername(List<Promotion> promotions, string username)
        {
            foreach(Promotion p in promotions)
            {

                //JObject jObject = (JObject)JsonConvert.DeserializeObject(CustomStuffRepository.Get(2).Data);
                //if ((int)jObject["promotionId"] == promotionId && ((JArray)jObject["usernames"]).Any(u => string.Equals(u.ToString().Trim(), username, StringComparison.CurrentCultureIgnoreCase)))
                //{
                //    return true;
                //}
            }
            return promotions;
        }*/
    }
}
