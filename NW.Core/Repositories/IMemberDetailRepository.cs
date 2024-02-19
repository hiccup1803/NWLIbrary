using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IMemberDetailRepository : IRepository<MemberDetail, int>
    {
        void InsertOrUpdate(int memberId, string key, string value);
        int MemberId(string key, string value);
    }
}
