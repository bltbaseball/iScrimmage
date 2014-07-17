using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    //public class TournamentMapping : ClassMap<Tournament>
    //{
    //    public TournamentMapping()
    //    {
    //        Table("Tournaments");
    //        Id(c => c.Id).GeneratedBy.Native();
    //        Map(c => c.CreatedOn);
    //        Map(c => c.IsActive);
    //        Map(c => c.Name);
    //        Map(c => c.Url);
    //    }
    //}

    //public class TournamentRegistrationMapping : ClassMap<TournamentRegistration>
    //{
    //    public TournamentRegistrationMapping()
    //    {
    //        Table("TournamentRegistrations");
    //        Id(c => c.Id).GeneratedBy.Native();
    //        Map(c => c.RegisteredOn);
    //        References(c => c.Payment);
    //        References(c => c.Tournament);
    //        References(c => c.Team);
    //    }
    //}
}