﻿using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Repositories
{
    public interface IGameCategoryRepository : IRepository<GameCategory, int>
    {
        IQueryable<Game> GamesByCategoryId(int categoryId, bool isMobile);
    }
}
