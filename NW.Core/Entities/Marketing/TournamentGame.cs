using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class TournamentGame : Entity<int>
    {
        public virtual int CasinoGameId { get; set; }
        public virtual int TournamentId { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int StatusType { get; set; }
        public virtual Game CasinoGame { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
