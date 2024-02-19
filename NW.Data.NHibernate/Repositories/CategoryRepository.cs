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
    public class CategoryRepository : Repository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ISession _session) : base(_session) { }

        public Category Category(string categoryAlias)
        {
            return GetAll().FirstOrDefault(c => c.FriendlyUrl == categoryAlias);
        }
    }
}
