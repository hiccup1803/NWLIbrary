using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class GameCategory : Entity<int>
    {
        public virtual int CategoryId { get; set; }
        public virtual int GameId { get; set; }
        public virtual bool Active { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public virtual Category Category { get; set; }
        public virtual Game Game { get; set; }
    }
}
