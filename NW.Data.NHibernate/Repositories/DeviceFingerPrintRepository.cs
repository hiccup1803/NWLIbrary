using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class DeviceFingerPrintRepository : Repository<DeviceFingerPrint, int>, IDeviceFingerPrintRepository
    {
        public DeviceFingerPrintRepository(ISession _session) : base(_session){ }

        public int GetStatusTypeByHash(string hash)
        {
            DeviceFingerPrint deviceFingerPrint = GetAll().FirstOrDefault(md => md.Hash == hash);
            DateTime date = DateTime.Now;
            if (deviceFingerPrint == null)
            {
                deviceFingerPrint = new DeviceFingerPrint() { Hash = hash, StatusType = (int)StatusType.Passive, CreateDate = date, UpdateDate = date };
                Insert(deviceFingerPrint);
            }
            return deviceFingerPrint.StatusType;
        }
        public DeviceFingerPrint DeviceFingerPrint(string hash)
        {
            return GetAll().FirstOrDefault(md => md.Hash == hash);
        }
    }
}
