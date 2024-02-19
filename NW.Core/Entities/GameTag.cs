using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    
    public class GameTag : Entity<int>
    {
        public virtual int GameId { get; set; }
        public virtual int TagId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual bool Active { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual Game Game { get; set; }
    }
}
