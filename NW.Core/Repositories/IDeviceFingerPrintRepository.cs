using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IDeviceFingerPrintRepository : IRepository<DeviceFingerPrint, int>
    {
        int GetStatusTypeByHash(string hash);
        DeviceFingerPrint DeviceFingerPrint(string hash);
    }
}
