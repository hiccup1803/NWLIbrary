using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Service;
using NHibernate;
using NW.Core.Model.Game;
using AutoMapper;

namespace NW.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        IRepository<Category, int> CategoryRepository { get; set; }

        public CategoryService(IRepository<Category, int> _categoryRepository, IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            CategoryRepository = _categoryRepository;
        }

        public virtual IList<CategoryModel> ChildCategories(int parentCategoryId)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {


                return CategoryRepository.GetAll().Where(c => c.ParentCasinoCategoryId == parentCategoryId && c.Active).OrderBy(c => c.DisplayOrder).ToList().Select(c => Mapper.Map<Category, CategoryModel>(c)).ToList();
            }
        }

        public virtual IList<CategoryModel> ChildCategories(int companyId, string casinoCategoryTemplateSystemName)
        {
            using (var uniOfWork = UnitOfWork.Current)
            {
                

                Category category = CategoryRepository.GetAll().Where(c => c.CategoryTemplate.SystemName == casinoCategoryTemplateSystemName && c.CompanyId == companyId && c.ParentCasinoCategoryId == 0 && c.Active).OrderBy(c => c.DisplayOrder).FirstOrDefault();
                return ChildCategories(category.Id);
            }
        }

        public virtual CategoryModel Category(int companyId, string friendlyUrl)
        {
            CategoryModel resultCategory;
            Category category;
            if (friendlyUrl == AllGamesCategory.FriendlyUrl)
                resultCategory = AllGamesCategory;
            else
            {
                using (var uniOfWork = UnitOfWork.Current)
                {
                    

                    category = CategoryRepository.GetAll().FirstOrDefault(c => c.CompanyId == companyId && c.FriendlyUrl == friendlyUrl);
                    resultCategory = Mapper.Map<Category, CategoryModel>(category);
                }
            }
            return resultCategory;
        }

        public virtual CategoryTemplateModel TopCategoryTemplate(int categoryId)
        {
            CategoryTemplateModel categoryTemplateModel = null;
            if (categoryId > 0)
            {
                using (var uniOfWork = UnitOfWork.Current)
                {


                    int topCategoryId;

                    int parentCategoryId = CategoryRepository.Get(categoryId).ParentCasinoCategoryId;
                    topCategoryId = categoryId;

                    while (parentCategoryId != 0)
                    {
                        Category tempCategory = CategoryRepository.Get(parentCategoryId);
                        parentCategoryId = tempCategory.ParentCasinoCategoryId;
                        topCategoryId = tempCategory.ParentCasinoCategoryId == 0 ? tempCategory.Id : topCategoryId;
                    }


                    categoryTemplateModel = Mapper.Map<CategoryTemplate, CategoryTemplateModel>(CategoryRepository.Get(topCategoryId).CategoryTemplate);
                }
            }
            return categoryTemplateModel;
        }
    }
}
