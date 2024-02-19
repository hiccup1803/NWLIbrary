using NW.Core.Entities;
using NW.Core.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model
{
    public class GamePagingListModel
    {
        public IList<GameModel> GameList { get; set; }
        public int GameCount { get; set; }
    }
}
