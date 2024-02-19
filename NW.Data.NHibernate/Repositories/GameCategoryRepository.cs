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
    public class GameCategoryRepository : Repository<GameCategory, int>, IGameCategoryRepository
    {
        public GameCategoryRepository(ISession _session) : base(_session) { }

        public IQueryable<Game> GamesByCategoryId(int categoryId, bool isMobile)
        {
            return GetAll().Where(gc => gc.CategoryId == categoryId && gc.Game.IsMobile == isMobile && gc.Active).OrderBy(gc => gc.DisplayOrder).Select(gc => gc.Game);
        }
    }
}
