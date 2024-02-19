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
    public class FAQRepository : Repository<FAQ, int>, IFAQRepository
    {
        public FAQRepository(ISession _session) : base(_session) { }

        public FAQ GetFAQ(string pageName, int companyId, string languageCode)
        {
            int lang = (int)Enum.Parse(typeof(Language), languageCode.ToUpperInvariant());

            IQueryable<FAQ> query = GetAll().Where(m => m.Title == pageName
                && m.CompanyId == companyId && m.LanguageId == lang);
            return query.FirstOrDefault();
        }

        public FAQ GetFAQ(int faqId, int companyId, string languageCode)
        {
            int lang = (int)Enum.Parse(typeof(Language), languageCode.ToUpperInvariant());
            IQueryable<FAQ> query = GetAll().Where(m => m.Id == faqId && m.CompanyId == companyId && m.LanguageId == lang);
            return query.FirstOrDefault();
        }


    }
    public enum FAQCategory
    {
        DEPOSIT = 1,
        WITHDRAWAL = 2,
        SECURITY = 3,
        RESPONSIBLE_GAMING = 4
    }   
}
