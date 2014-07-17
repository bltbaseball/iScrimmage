using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class DivisionMapping : ClassMap<Division>
    {
        public DivisionMapping()
        {
            Table("Divisions");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name).Not.Nullable().Unique();
            Map(c => c.MaxAge).Not.Nullable();
            Map(c => c.CreatedOn).Not.Nullable();
            HasManyToMany(c => c.Leagues)
                    .Cascade.All()
                    .Inverse()
                    .Table("LeagueDivisions");
            HasMany(c => c.Teams);            
        }
    }
}