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
    


    public class GameRepository : Repository<Game, int>, IGameRepository
    {
        public GameRepository(ISession _session) : base(_session) { }

        public Game Game(string seoUrl)
        {
            return GetAll().FirstOrDefault(g => g.Active == true && g.Alias == seoUrl);
        }

        public Game Game(int id)
        {
            return GetAll().FirstOrDefault(g => g.Active == true && g.Id == id);
        }

        public Game GameByVoltronGameId(int voltronGameId)
        {
            return GetAll().FirstOrDefault(g => g.Active == true && g.VoltronGameId == voltronGameId);
        }


        public Game GameButNotThatId(string seoUrl, int id)
        {
            return GetAll().FirstOrDefault(g => g.Alias == seoUrl && g.ResourceName.StartsWith("Evolution."));
        }

        public Game MobileGameByDesktopAlias(string alias, string vendor)
        {
            return GetAll().FirstOrDefault(g => g.Alias.Contains(alias) && g.IsMobile == true && g.Vendor == vendor && g.Active == true);
        }

        public Game DesktopGameByMobileAlias(string alias, string vendor)
        {
            alias = alias.Replace("-mobile", string.Empty);
            alias = alias.Replace("-touch", string.Empty);
            return GetAll().FirstOrDefault(g => g.Alias.Contains(alias) && g.IsMobile == false && g.Vendor == vendor && g.Active == true);
        }
    }
}
