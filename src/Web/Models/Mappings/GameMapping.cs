using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class GameMapping : ClassMap<Game>
    {
        public GameMapping()
        {
            Table("Games");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            References(c => c.HomeTeam);
            References(c => c.AwayTeam);
            Map(c => c.GameDate).Not.Nullable();
            References(c => c.Location);
            Map(c => c.Innings);
            Map(c => c.HomeTeamScore);
            Map(c => c.AwayTeamScore);
            References(c => c.CreatedBy);
            Map(c => c.CreatedOn).Not.Nullable();
            References(c => c.PlateUmpire).Nullable();
            References(c => c.FieldUmpire).Nullable();
            References(c => c.Bracket).Nullable();
            Map(c => c.BracketBracket).Nullable();
            Map(c => c.BracketPosition).Nullable();
            Map(c => c.Status);
            Map(c => c.Field);
            References(c => c.TeamToConfirmGame);
            References(c => c.TeamToRequestGame);
            References(c => c.Division);
            //UmpireFee
            HasMany(c => c.PlayerGameStats).Inverse().Cascade.Delete();
        }
    }
}