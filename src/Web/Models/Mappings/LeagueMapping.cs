using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class LeagueMapping : ClassMap<League>
    {
        public LeagueMapping()
        {
            Table("Leagues");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name).Not.Nullable();
            Map(c => c.Url);
            Map(c => c.StartDate).Not.Nullable();
            Map(c => c.EndDate);
            Map(c => c.RegistrationStartDate).Not.Nullable();
            Map(c => c.RegistrationEndDate).Not.Nullable();
            Map(c => c.RosterLockedOn).Nullable();
            Map(c => c.IsActive);
            Map(c => c.WaiverRequired);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.Type);
            Map(c => c.HtmlDescription);
            Map(c => c.MinimumDatesAvailable);

            HasMany(c => c.Teams);
            HasManyToMany(c => c.Divisions)
                    .Cascade.All()
                    .Table("LeagueDivisions");           
        }
    }
}