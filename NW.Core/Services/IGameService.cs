using NW.Core.Contracts.Game;
using NW.Core.Entities;
using NW.Core.Model;
using NW.Core.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IGameService
    {
        int[] ClosedGames(string domain, bool isProduction);
        GamePagingListModel GamesByCategoryId(string domain, bool isProduction, int categoryId, string countryCode, bool isMobile, int skip, int take, bool needToGetClosedGames);
        IList<GameModel> FavouritedGames(int memberId, string countryCode, bool isMobile, int skip, int take);
        IList<GameModel> GamesByResourceName(string domain, IList<string> resourceNames, string countryCode, bool isMobile, int skip, int take);
        int GameCount(int categoryId, string countryCode, bool isMobile);
        int FavouritedGameCount(int memberId, string countryCode, bool isMobile);
        int SearchGameCount(string searchTerm, string countryCode, bool isMobile);
        GameModel Game(string seoURL);
        GameModel Game(int id);
        GameModel GameByVoltronGameId(int voltronGameId);
        IList<GameModel> Games(IList<int> gameIds);
        IList<GameModel> GetAllActiveGameList(string domain);
        WinnerListResult GetWinners(string domain, bool isProduction);

        GameModel GetMobileVersion(GameModel game);
        GameModel GetDesktopVersion(GameModel game);

    }
}