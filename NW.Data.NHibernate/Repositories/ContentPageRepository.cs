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
    public class ContentPageRepository : Repository<ContentPage, int>, IContentPageRepository
    {
        public ContentPageRepository(ISession _session) : base(_session) { }

        public ContentPage GetContent(string pageName, int companyId, string languageCode)
        {
            int lang = (int)Enum.Parse(typeof(Language), languageCode.ToUpperInvariant());

            IQueryable<ContentPage> query = GetAll().Where(m => m.PageName == pageName && m.CompanyId == companyId && m.LanguageId == lang);
            return query.FirstOrDefault();
        }

        public ContentPage GetContent(int pageId, int companyId, string languageCode)
        {
            int lang = (int)Enum.Parse(typeof(Language), languageCode.ToUpperInvariant());
            IQueryable<ContentPage> query = GetAll().Where(m => m.PageId == pageId && m.CompanyId == companyId && m.LanguageId == lang);
            return query.FirstOrDefault();
        }
    }

    public enum Language
    {
        TR = 1, EN = 2, FR = 3, SE = 4, DE = 5, ES = 6, JA = 10
    }

    public enum LanguageEqualDB
    {
        TR = 9, EN = 1, JA = 10
    }
}
