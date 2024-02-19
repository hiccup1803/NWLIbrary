using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class GameDetail : Entity<int>
    {
        public virtual Game Game { get; set; }
        public virtual int GameId { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}
