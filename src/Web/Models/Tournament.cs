using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    //public class Tournament
    //{
    //    public virtual int Id { get; set; }
    //    public virtual string Name { get; set; }
    //    public virtual string Url { get; set; }
    //    //public virtual DateTime Date { get; set; }
    //    public virtual bool IsActive { get; set; }
    //    //public virtual IList<Division> Division { get; set; }
    //    //public virtual IList<Team> Teams { get; set; }
    //    public virtual DateTime CreatedOn { get; set; }
    //    //public virtual Double Fee { get; set; }

    //    public static Tournament GetTournamentById(int id)
    //    {
    //        var session = MvcApplication.SessionFactory.GetCurrentSession();
    //        return session.Get<Tournament>(id);
    //    }
    //}

    //public class TournamentRegistration
    //{
    //    public virtual int Id { get; set; }
    //    public virtual Tournament Tournament { get; set; }
    //    public virtual Team Team { get; set; }
    //    public virtual PayPalPayment Payment { get; set; }
    //    public virtual DateTime? RegisteredOn { get; set; }

    //    public static TournamentRegistration GetTournamentRegistrationById(int id)
    //    {
    //        var session = MvcApplication.SessionFactory.GetCurrentSession();
    //        return session.Get<TournamentRegistration>(id);
    //    }

    //    public static TournamentRegistration GetTournamentregistrationForTeam(Tournament tournament, Team team)
    //    {
    //        var session = MvcApplication.SessionFactory.GetCurrentSession();
    //        return session.QueryOver<TournamentRegistration>().Where(t => t.Team == team && t.Tournament == tournament).SingleOrDefault();
    //    }
    //}
}