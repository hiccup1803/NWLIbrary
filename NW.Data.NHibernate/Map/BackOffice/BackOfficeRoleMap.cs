using FluentNHibernate.Mapping;
using NW.Core.Entities.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Map.BackOffice
{
    public class BackOfficeRoleMap : ClassMap<BackOfficeRole>
    {
        public BackOfficeRoleMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.StatusType);
            Map(x => x.CreateDate);

            Table("BackOfficeRole");
        }
    }
}
