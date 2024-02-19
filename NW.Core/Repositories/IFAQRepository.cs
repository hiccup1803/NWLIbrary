using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IFAQRepository : IRepository<FAQ, int>
    {
        FAQ GetFAQ(string pageName, int companyId, string languageCode);
        FAQ GetFAQ(int faqId, int companyId, string languageCode);
    }

}
