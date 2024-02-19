using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Repositories;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class MemberRepository : Repository<Member, int>, IMemberRepository
    {
        private int[] StatusTypeList = new int[] { (int)StatusType.Active, (int)StatusType.BonusAbuser, (int)StatusType.KYC };
        private int[] StatusTypeList2 = new int[] { (int)StatusType.Active, (int)StatusType.Passive, (int)StatusType.UnWanted, (int)StatusType.BonusAbuser, (int)StatusType.KYC };


        public MemberRepository(ISession _session) : base(_session) { }

        public Member Member(int companyId, string email, params int[] statusTypes)
        {
            IQueryable<Member> query = GetAll().Where(m => m.CompanyId == companyId && (m.Email == email || m.Username == email)).OrderByDescending(m => m.CreateDate);
            if (statusTypes != null && statusTypes.Count() > 0)
                query = query.Where(m => statusTypes.Contains(m.StatusType));


            return query.FirstOrDefault();
        }


        public bool ValidateStep1RegisterEmail(int companyId, string email)
        {
            return !GetAll().Any(m => m.CompanyId == companyId && m.Email == email && StatusTypeList2.Contains(m.StatusType));
        }

        public bool ValidateStep1RegisterUsername(int companyId, string username)
        {
            return !GetAll().Any(m => m.CompanyId == companyId && m.Username == username && StatusTypeList2.Contains(m.StatusType));
        }

        public bool ValidateStep1Password(int companyId, string password, string username)
        {
            return !password.ToLowerInvariant().Contains(username.ToLowerInvariant());
        }

        public bool ValidatePassword(int companyId, string password, string firstname, string lastname, string username)
        {
            return !password.ToLowerInvariant().Contains(firstname.ToLowerInvariant())
                && !password.ToLowerInvariant().Contains(lastname.ToLowerInvariant())
                && !password.ToLowerInvariant().Contains(username.ToLowerInvariant());
        }

        public Member ActiveMember(int companyId, string emailOrUsername)
        {
            IQueryable<Member> query = GetAll().Where(m => m.CompanyId == companyId && (m.Email == emailOrUsername || m.Username == emailOrUsername) && StatusTypeList.Contains(m.StatusType)).OrderByDescending(m => m.CreateDate);


            return query.FirstOrDefault();
        }

        public Member Member(int companyId, string emailOrUsername)
        {
            IQueryable<Member> query = GetAll().Where(m => m.CompanyId == companyId && (m.Email == emailOrUsername || m.Username == emailOrUsername)).OrderByDescending(m => m.CreateDate);


            return query.FirstOrDefault();
        }
        public Member Member(int companyId, int memberId)
        {
            IQueryable<Member> query = GetAll().Where(m => m.CompanyId == companyId && m.Id == memberId);


            return query.FirstOrDefault();
        }
    }
}
