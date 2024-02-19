using NW.Core.Entities;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IMemberRepository : IRepository<Member, int>
    {
        Member ActiveMember(int companyId, string emailOrUsername);
        Member Member(int companyId, string emailOrUsername);
        Member Member(int companyId, int memberId);
        Member Member(int companyId, string emailOrUsername, params int[] statusType);
        bool ValidateStep1RegisterEmail(int companyId, string email);
        bool ValidateStep1RegisterUsername(int companyId, string username);
        bool ValidateStep1Password(int companyId, string password, string username);
        bool ValidatePassword(int companyId, string password, string firstname, string lastname, string username);
    }
}