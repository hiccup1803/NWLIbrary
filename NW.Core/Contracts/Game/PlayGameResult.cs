using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Contracts.Game
{
    public class PlayGameResult
    {
        public bool IsSuccess { get; set; }
        public string Message{ get; set; }
        public int ResponseCode { get; set; }

        
        public string GameLaunchUrl { get; set; }

        public string BackgroundImage { get; set; }
        public string Description { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        
    }
}
