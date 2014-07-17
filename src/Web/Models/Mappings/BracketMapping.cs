using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class BracketMapping : ClassMap<Bracket>
    {
        public BracketMapping()
        {
            Table("Brackets");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name);
            Map(c => c.CreatedOn);
            References(c => c.League);
            References(c => c.Division);
            HasMany(c => c.Standings).Cascade.None();
            HasMany(c => c.BracketGenerator).Cascade.Delete();
        }
    }

    public class BracketTeamMapping : ClassMap<BracketTeam>
    {
        public BracketTeamMapping()
        {
            Table("BracketTeams");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Standing);
            References(c => c.Bracket);
            References(c => c.Team);
        }
    }

    public class BracketGeneratorMapping : ClassMap<BracketGenerator>
    {
        public BracketGeneratorMapping()
        {
            Table("BracketGenerator");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            References(c => c.Bracket);
            Map(c => c.Sequence);
            Map(c => c.Team1);
            Map(c => c.Team2);
            Map(c => c.BracketPosition).Column("Position");
            Map(c => c.BracketBracket).Column("Bracket");
            Map(c => c.GameNumber);
        }
    }
}