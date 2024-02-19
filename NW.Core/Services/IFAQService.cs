using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IFAQService
    {
        FAQ GetFAQ(int id);
        FAQ InsertFAQ(FAQ faq);
        FAQ GetFAQ(string pageName, int companyId, string languageCode);
        FAQ GetFAQ(int pageId, int companyId, string languageCode);
        FAQ UpdateFAQ(FAQ faq);
        PagingModel<FAQ> FAQs(int pageIndex, int pageSize);
        PagingModel<FAQ> FAQs(int pageIndex, int pageSize, int companyId);
    }
}
