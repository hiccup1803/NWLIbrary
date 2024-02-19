using NHibernate;
using NW.Core.Entities;
using NW.Core.Entities.Campaign;
using NW.Core.Repositories;
using NW.Core.Services.Marketing;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Marketing
{
    public class EventCampaignService : BaseService, IEventCampaignService
    {
        IRepository<EventType, int> EventTypeRepository { get; set; }
        IRepository<ActionType, int> ActionTypeRepository { get; set; }
        IRepository<EventHistory, int> EventHistoryRepository { get; set; }
        IRepository<EventCampaign, int> EventCampaignRepository { get; set; }
        IRepository<EventCampaignHistory, int> EventCampaignHistoryRepository { get; set; }
        public EventCampaignService(IRepository<EventType, int> _eventTypeRepositry, IRepository<ActionType, int> _actionTypeRepository, IRepository<EventHistory, int> _eventHistoryRepository, IRepository<EventCampaign, int> _eventCampaignRepository, IRepository<EventCampaignHistory, int> _eventCampaignHistoryRepository, IUnitOfWork _unitOfWork, ISession _session)
            : base(_unitOfWork, _session)
        {
            EventTypeRepository = _eventTypeRepositry;
            ActionTypeRepository = _actionTypeRepository;
            EventHistoryRepository = _eventHistoryRepository;
            EventCampaignRepository = _eventCampaignRepository;
            EventCampaignHistoryRepository = _eventCampaignHistoryRepository;
        }
        #region EventType
        public IList<EventType> EventTypes() {
            return EventTypeRepository.GetAll().ToList();
        }
        #endregion
        #region ActionType
        public IList<ActionType> ActionTypes()
        {
            return ActionTypeRepository.GetAll().ToList();
        }
        #endregion
        #region EventHistory
        public EventHistory InsertEventHistory(EventHistory eventHistory)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    eventHistory.CreateDate = DateTime.Now;
                    eventHistory = EventHistoryRepository.Insert(eventHistory);
                    unitOfWork.Commit(transaction);
                    return eventHistory;
                }
            }
        }
        #endregion
        #region Campaign
        public EventCampaign EventCampaign(int id)
        {
            return EventCampaignRepository.Get(id);
        }
        public PagingModel<EventCampaign> EventCampaigns(int pageIndex, int pageSize)
        {
            PagingModel<EventCampaign> pagingModel = new PagingModel<EventCampaign>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = EventCampaignRepository.GetAll().Count();
                    pagingModel.ItemList = Session.QueryOver<EventCampaign>()
                            .OrderBy(ec => ec.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public PagingModel<EventCampaign> EventCampaigns(int pageIndex, int pageSize, int companyId)
        {
            PagingModel<EventCampaign> pagingModel = new PagingModel<EventCampaign>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = EventCampaignRepository.GetAll().Where(ec => ec.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<EventCampaign>()
                            .Where(ec => ec.CompanyId == companyId)
                            .OrderBy(ec => ec.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public EventCampaign InsertEventCampaign(EventCampaign eventCampaign)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    eventCampaign.CreateDate = DateTime.Now;
                    eventCampaign = EventCampaignRepository.Insert(eventCampaign);
                    unitOfWork.Commit(transaction);
                    return eventCampaign;
                }
            }
        }
        public EventCampaign UpdateEventCampaign(EventCampaign eventCampaign)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    eventCampaign = EventCampaignRepository.Update(eventCampaign);
                    unitOfWork.Commit(transaction);
                    return eventCampaign;
                }
            }
        }
        #endregion
        #region EventCampaignHistory
        public EventCampaignHistory CampaignHistory(int id)
        {
            return EventCampaignHistoryRepository.Get(id);
        }
        public PagingModel<EventCampaignHistory> CampaignHistorys(int pageIndex, int pageSize, int eventCampaignId)
        {
            PagingModel<EventCampaignHistory> pagingModel = new PagingModel<EventCampaignHistory>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = EventCampaignHistoryRepository.GetAll().Where(ec => ec.EventCampaignId == eventCampaignId).Count();
                    pagingModel.ItemList = Session.QueryOver<EventCampaignHistory>()
                            .Where(ec => ec.EventCampaignId == eventCampaignId)
                            .OrderBy(ec => ec.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public IList<EventCampaignHistory> CampaignHistorysForMember(int pageIndex, int pageSize, int eventCampaignId, int memberId)
        {
            return EventCampaignHistoryRepository.GetAll().Where(ech => ech.EventCampaignId == eventCampaignId && ech.MemberId == memberId).ToList();
        }
        public EventCampaignHistory InsertEventCampaignHistory(EventCampaignHistory eventCampaignHistory)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    eventCampaignHistory.CreateDate = DateTime.Now;
                    eventCampaignHistory = EventCampaignHistoryRepository.Insert(eventCampaignHistory);
                    unitOfWork.Commit(transaction);
                    return eventCampaignHistory;
                }
            }
        }
        #endregion
    }
}