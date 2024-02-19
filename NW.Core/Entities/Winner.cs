using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Winner
    {

        public decimal Amount { get; set; }

        public string FirstName { get; set; }
        public string LastName{ get; set; }

        public string Username { get; set; }
        public string MaskedUsername { get; set; }
        public string Game { get; set; }
        public int GameId { get; set; }

        public string GameAlias { get; set; }
        public string GameThumbnail { get; set; }
        public string Vendor { get; set; }

    }
}
