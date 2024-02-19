using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;

namespace NW.Core.Services
{
    public interface IContentPageService
    {
        ContentPage ContentPage(int id);
        ContentPage InsertContentPage(ContentPage contentPage);
        ContentPage GetContent(string pageName, int companyId, string languageCode);
        ContentPage GetContent(int pageId, int companyId, string languageCode);
        ContentPage UpdateContentPage(ContentPage contentPage);
        PagingModel<ContentPage> ContentPages(int pageIndex, int pageSize);
        PagingModel<ContentPage> ContentPages(int pageIndex, int pageSize, int companyId);
    }
}
