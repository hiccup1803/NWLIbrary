using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IMemberDeviceFingerPrintRepository : IRepository<MemberDeviceFingerPrint, int>
    {
        void InsertOrUpdate(int memberId, string ip, bool? isLoggedIn, string hash, int memberDeviceFingerPrintStatusType);
        string GetLastHash(int memberId);
        int GetCountHashByMemberId(int memberId);
        int GetCountMemberIdByHash(string hash);
    }
}
