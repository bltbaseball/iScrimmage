using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class TeamMapping : ClassMap<Team>
    {
        public TeamMapping()
        {
            Table("Teams");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name).Not.Nullable();
            Map(c => c.Url);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.IsLookingForPlayers);
            Map(c => c.HtmlDescription);
            References(c => c.Class);
            References(c => c.Division);
            References(c => c.League);
            References(c => c.Location);
            HasMany(c => c.DatesAvailable).Cascade.None();
            HasMany(c => c.Players).Cascade.None();
            HasManyToMany(c => c.Managers)
                .Table("TeamManagers")
                .Cascade.None()
                .LazyLoad();

            HasManyToMany(c => c.Coaches)
                .Table("TeamCoaches")
                .Cascade.None()
                .LazyLoad();
        }
    }

    public class AvailableDateMapping : ClassMap<AvailableDates>
    {
        public AvailableDateMapping()
        {
            Table("AvailableDates");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            References(c => c.Team).Not.Nullable();
            References(c => c.Location).Not.Nullable();
            Map(c => c.Date).Not.Nullable();
            Map(c => c.GameScheduled).Not.Nullable();
            Map(c => c.IsHome).Not.Nullable();
            Map(c => c.IsAway).Not.Nullable();
            Map(c => c.DistanceFromLocation).Nullable();
        }
    }
}