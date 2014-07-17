using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class BracketResultMapping : ClassMap<BracketResult>
    {
        public BracketResultMapping()
        {
            Id(c => c.Id).GeneratedBy.Identity();
            Map(c => c.Team1);
            Map(c => c.Team2);
            Map(c => c.Bracket);
            Map(c => c.Position);
            Map(c => c.GameNumber);
        }
    }
}