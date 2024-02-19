﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NW.Core.Entities;

namespace NW.Data.NHibernate.Map.Member
{
    public class MemberDeviceTokenMap : ClassMap<MemberDeviceToken>
    {
        public MemberDeviceTokenMap()
        {
            Id(l => l.Id);
            Map(l => l.MemberId);
            Map(l => l.UserAgent);
            Map(l => l.IP);
            Map(l => l.CountryCode);
            Map(l => l.CreateDate);
            Map(l => l.LastUsedDate);
        }
    }
}