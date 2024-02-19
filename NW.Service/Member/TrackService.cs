using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using NW.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Enum;
using NHibernate;

namespace NW.Services
{
    public class TrackService : BaseService, ITrackService
    {
        IRepository<Track, Guid> TrackRepository { get; set; }
        public TrackService(IRepository<Track, Guid> _trackRepository, IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            TrackRepository = _trackRepository;
        }

        public void InsertTrack(ActionType actionType, int companyId, string sessionId, string userIdentity, string value, string ip, string extraFields)
        {
            //Logging.Logger logger = Logging.Logger.UserTrack;
            //logger.Track(string.Format("{0},{1},{2},{3},{4},{5},{6}", actionType, companyId, sessionId, userIdentity, value, ip, extraFields));
        }
    }
}
