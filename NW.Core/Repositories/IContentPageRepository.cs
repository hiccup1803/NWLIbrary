using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Repositories
{
    public interface IContentPageRepository: IRepository<ContentPage, int>
    {
        ContentPage GetContent(string pageName, int companyId, string languageCode);
        ContentPage GetContent(int pageId, int companyId, string languageCode);
    }
}
