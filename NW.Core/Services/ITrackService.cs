using NW.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ITrackService
    {
        void InsertTrack(ActionType actionType, int companyId, string sessionId, string userIdentity, string value, string ip, string extraFields);
    }
}
