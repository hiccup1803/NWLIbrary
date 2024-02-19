using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class MemberDetailRepository : Repository<MemberDetail, int>, IMemberDetailRepository
    {
        public MemberDetailRepository(ISession _session) : base(_session) { }

        public int MemberId(string key, string value)
        {
            return GetAll().FirstOrDefault(md => md.Key == key && md.Value == value).MemberId;
        }

        public void InsertOrUpdate(int memberId, string key, string value)
        {
            MemberDetail memberDetail = GetAll().FirstOrDefault(md => md.MemberId == memberId && md.Key == key);
            DateTime date = DateTime.Now;
            if (memberDetail != null)
            {
                memberDetail.Value = value;
                memberDetail.UpdateDate = date;
                Update(memberDetail);
            }
            else
            {
                Insert(new MemberDetail() { MemberId = memberId, Key = key, Value = value, CreateDate = date, UpdateDate = date });
            }
        }
    }
}
