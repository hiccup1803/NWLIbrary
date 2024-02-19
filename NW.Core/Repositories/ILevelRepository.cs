using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface ILevelRepository : IRepository<Level, int>
    {
        Level Level(string systemName);
        IQueryable<Level> LevelList();
    }
}
