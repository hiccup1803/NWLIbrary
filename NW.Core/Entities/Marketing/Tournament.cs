using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities.Marketing
{
    public class Tournament : Entity<int>
    {
        public virtual string GameTypeId { get; set; }
        public virtual int CompanyId { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual int ThirdPartyTournamentId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Terms { get; set; }
        public virtual string Notes { get; set; }
        public virtual int TotalPrize { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual string PrizeList { get; set; }
        public virtual string RandomPrizeList { get; set; }
        public virtual string ResultList { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool IsVip { get; set; }
        public virtual int TournamentType { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual int StatusType { get; set; }
        public virtual IList<TournamentGame> TournamentGames { get; set; }
    }
}
