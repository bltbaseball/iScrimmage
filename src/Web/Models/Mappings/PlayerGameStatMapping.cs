using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class PlayerGameStatMapping : ClassMap<PlayerGameStat>
    {

        public PlayerGameStatMapping()
        {
            Table("PlayerGameStats");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.InningsPitched).Nullable();
            Map(c => c.InningsOuts).Nullable();
            Map(c => c.PitchesThrown).Nullable();
            References(c => c.TeamPlayer).Not.Nullable();
            References(c => c.Game).Not.Nullable();
        }
    }
}