using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class TeamClassMapping : ClassMap<TeamClass>
    {
        public TeamClassMapping()
        {
            Table("TeamClasses");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name).Not.Nullable();
            Map(c => c.Handicap);
            Map(c => c.CreatedOn).Not.Nullable();
        }
    }
}