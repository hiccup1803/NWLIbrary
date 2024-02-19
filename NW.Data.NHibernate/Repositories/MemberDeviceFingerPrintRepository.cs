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
    public class MemberDeviceFingerPrintRepository : Repository<MemberDeviceFingerPrint, int>, IMemberDeviceFingerPrintRepository
    {
        public MemberDeviceFingerPrintRepository(ISession _session) : base(_session) { }

        public int GetCountMemberIdByHash(string hash)
        {
            return GetAll().Count(mdfp => mdfp.Hash == hash && mdfp.CreateDate >= DateTime.UtcNow.AddDays(-5));
        }

        public int GetCountHashByMemberId(int memberId)
        {
            return GetAll().Count(mdfp => mdfp.MemberId == memberId);
        }

        public string GetLastHash(int memberId)
        {
            MemberDeviceFingerPrint memberDeviceFingerPrint = GetAll().OrderByDescending(md => md.CreateDate).FirstOrDefault(md => md.MemberId == memberId && md.StatusType == (int)StatusType.Active);
            return memberDeviceFingerPrint != null ? memberDeviceFingerPrint.Hash : string.Empty;
        }

        public void InsertOrUpdate(int memberId, string ip, bool? isLoggedIn, string hash, int memberDeviceFingerPrintStatusType)
        {
            MemberDeviceFingerPrint memberDeviceFingerPrint = GetAll().FirstOrDefault(md => md.MemberId == memberId && md.Hash == hash);
            DateTime date = DateTime.Now;
            if (memberDeviceFingerPrint != null)
            {
                if (memberDeviceFingerPrintStatusType == (int)MemberDeviceFingerPrintStatusType.Force2FA)
                    memberDeviceFingerPrint.UpdateDate = date.AddHours(-3);
                else if(memberDeviceFingerPrint.StatusType != (int)MemberDeviceFingerPrintStatusType.Force2FA)
                    memberDeviceFingerPrint.UpdateDate = date;

                memberDeviceFingerPrint.IP = ip;
                memberDeviceFingerPrint.IsLoggedIn = isLoggedIn;
                Update(memberDeviceFingerPrint);
            }
            else
            {
                Insert(new MemberDeviceFingerPrint() { MemberId = memberId, Hash = hash, StatusType = memberDeviceFingerPrintStatusType, CreateDate = date, UpdateDate = date, IP = ip, IsLoggedIn = isLoggedIn });
            }
        }
    }
}
