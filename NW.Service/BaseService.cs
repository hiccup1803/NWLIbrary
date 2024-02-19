using NHibernate;
using NW.Core.Entities;
using NW.Core.Model.Game;
using NW.Core.Work;
using NW.Data.NHibernate.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service
{
    public class BaseService
    {
        public CategoryModel AllGamesCategory
        {
            get
            {
                return new CategoryModel() { Id = 0, FriendlyUrl = "all-games", Active = true, ResourceKey = "CasinoCategory.AllGames", CategoryTemplate = new CategoryTemplateModel() };
            }
        }
        protected UnitOfWork UnitOfWork { get; set; }
        protected ISession Session { get; set; }
        public BaseService(IUnitOfWork _unitOfWork, ISession _session)
        {
            UnitOfWork = (UnitOfWork)_unitOfWork;  
            Session = _session;
        }
    }
}
