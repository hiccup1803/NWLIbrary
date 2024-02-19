using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Entities
{
    public class Country : Entity<int>
    {
        public virtual string Name { get; set; }
        public virtual string A2Code { get; set; }
        public virtual string A3Code { get; set; }

        public virtual IEnumerable<Game> RestrictedGames { get; set; }
    }
}
