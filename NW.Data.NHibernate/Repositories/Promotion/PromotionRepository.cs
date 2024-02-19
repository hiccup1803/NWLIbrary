using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;

namespace NW.Data.NHibernate.Repositories
{
    public class PromotionRepository : Repository<Promotion, int>, IPromotionRepository
    {

        public PromotionRepository(ISession _session) : base(_session) { }

        public Promotion Promotion(string categoryAlias)
        {
            throw new NotImplementedException();
        }
    }
}
