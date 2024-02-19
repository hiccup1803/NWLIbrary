using NW.Core.Entities;
using NW.Core.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ICategoryService
    {
        CategoryModel AllGamesCategory { get; }
        IList<CategoryModel> ChildCategories(int parentCategoryId);
        IList<CategoryModel> ChildCategories(int companyId, string casinoCategoryTemplateSystemName);
        CategoryModel Category(int companyId, string friendlyUrl);
        CategoryTemplateModel TopCategoryTemplate(int categoryId);
    }
}
