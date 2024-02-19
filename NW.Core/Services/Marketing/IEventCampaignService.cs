using NW.Core.Entities;
using NW.Core.Entities.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Marketing
{
    public interface IEventCampaignService
    {
        #region EventType
        IList<EventType> EventTypes();
        #endregion
        #region ActionType
        IList<ActionType> ActionTypes();
        #endregion
        #region EventHistory
        EventHistory InsertEventHistory(EventHistory eventHistory);
        #endregion
        #region Campaign
        EventCampaign EventCampaign(int id);
        PagingModel<EventCampaign> EventCampaigns(int pageIndex, int pageSize);
        PagingModel<EventCampaign> EventCampaigns(int pageIndex, int pageSize, int companyId);
        EventCampaign InsertEventCampaign(EventCampaign eventCampaign);
        EventCampaign UpdateEventCampaign(EventCampaign eventCampaign);
        #endregion
        #region EventCampaignHistory
        EventCampaignHistory CampaignHistory(int id);
        PagingModel<EventCampaignHistory> CampaignHistorys(int pageIndex, int pageSize, int eventCampaignId);
        IList<EventCampaignHistory> CampaignHistorysForMember(int pageIndex, int pageSize, int eventCampaignId, int memberId);
        EventCampaignHistory InsertEventCampaignHistory(EventCampaignHistory eventCampaignHistory);
        #endregion
    }
}
