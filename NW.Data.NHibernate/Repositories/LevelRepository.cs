using NHibernate;
using NW.Core.Entities;
using NW.Core.Enum;
using NW.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Repositories
{
    public class LevelRepository : Repository<Level, int>, ILevelRepository
    {
        public LevelRepository(ISession _session) : base(_session) { }
        public Level Level(string systemName)
        {
            return LevelList().FirstOrDefault(l => l.SystemName == systemName);
        }

        public IQueryable<Level> LevelList()
        {
            return GetAll().Where(l => l.StatusType == (int)StatusType.Active);
        }
    }
}
