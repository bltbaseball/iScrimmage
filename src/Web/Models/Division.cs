using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// Group of teams arranged by age groups that compete against each other. Shared across leagues and teams.
    /// </summary>
    public class Division
    {
        public virtual int Id { get; set; }

        /// <summary>
        /// Name of the Division. (e.g., Boys 9U)
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Maximum age for players in this division. Maximum age and League Start Date determine Minimum DOB for players.
        /// </summary>
        public virtual int MaxAge { get; set; }

        //public virtual List<League> Leagues { get; set; }
        /// <summary>
        /// League the division is assigned to. TODO: Can a division be in multiple leagues? (Is Summer 2012 Boys 9U the same as Summer 2013 Boys 9U?)
        /// </summary>
        public virtual IList<League> Leagues { get; set; }

        /// <summary>
        /// Teams assigned to the Division.
        /// </summary>
        public virtual IList<Team> Teams { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        public static Division GetDivisionById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Division>(id);
        }

        public static IList<Division> GetAllDivisions()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Division>().OrderBy(l => l.MaxAge).Desc.List();
        }
    }
}