using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Game
{
    public class GameModel
    {
        public int Id { get; set; }
        public int VoltronGameId { get; set; }
        public int CasinoGameTypeId { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string LogoURL { get; set; }
        public string ThumbnailURL { get; set; }
        public string ThumbnailHoverURL { get; set; }
        public string ImageURL { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ResourceName { get; set; }
        public bool IsMobile { get; set; }
        public bool IsNewGame { get; set; }
        public string Vendor { get; set; }

    }
}
