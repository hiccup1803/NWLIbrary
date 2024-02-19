using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IGameRepository : IRepository<Game, int>
    {
        Game Game(string seoUrl);
        Game Game(int id);
        Game GameByVoltronGameId(int voltronGameId);
        Game GameButNotThatId(string seoUrl, int id);


        Game MobileGameByDesktopAlias(string alias, string vendor);

        Game DesktopGameByMobileAlias(string alias, string vendor);
    }
}
