using NW.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMemberSegmentContainerService
    {
        int[] GetUsernameListByQuery(string query, int? interval, int? memberId = null);
    }
}
