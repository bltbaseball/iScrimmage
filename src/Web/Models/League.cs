using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public enum LeagueType
    {
        League,
        Tournament,
        Scrimmage,
        Other
    }

    /// <summary>
    /// A League or Tournament.
    /// </summary>
    public class League
    {
        public virtual int Id { get; set; }

        /// <summary>
        /// Name of the league. (e.g., Summer 2013)
        /// </summary>
        public virtual string Name { get; set; }

        public virtual string HtmlDescription { get; set; }

        /// <summary>
        /// Web site URL.
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Opening date of the league. Will determine minimum DOB for players.
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Closing date of the league. 
        /// </summary>
        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// First date registration is available.
        /// </summary>
        public virtual DateTime RegistrationStartDate { get; set; }

        /// <summary>
        /// Last date registration is available.
        /// </summary>
        public virtual DateTime RegistrationEndDate { get; set; }

        /// <summary>
        /// Whether the league is currently active. (May be automatically determined via EndDate as well.)
        /// </summary>
        public virtual bool IsActive { get; set; }

        public virtual bool WaiverRequired { get; set; }

        //public virtual List<TeamClass> Classes { get; set; }

        /// <summary>
        /// Divisions available to this league.
        /// </summary>
        public virtual IList<Division> Divisions { get; set; }

        /// <summary>
        /// Teams assigned to this league.
        /// </summary>
        public virtual IList<Team> Teams { get; set; }

        /// <summary>
        /// Date league was created.
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Date that the team rosters become locked.
        /// </summary>
        public virtual DateTime? RosterLockedOn { get; set; }

        public virtual LeagueType Type { get; set; }

        public virtual int? MinimumDatesAvailable { get; set; }

        //public virtual Double Fee { get; set; }

        public static League GetLeagueById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<League>(id);
        }

        public static League GetActiveTournamentById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<League>().Where(c => c.Id == id && c.IsActive == true && c.Type == LeagueType.Tournament).SingleOrDefault();
        }

        public static IList<League> GetAllLeagues()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<League>().Where(c =>  c.IsActive == true && c.Type != LeagueType.Other).OrderBy(i => i.CreatedOn).Desc.List();
        }

        public static IList<League> GetAllOther()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<League>().Where(c => c.IsActive == true && c.Type == LeagueType.Other).OrderBy(i => i.CreatedOn).Desc.List();
        }

        public static String LeagueNameFromGame(Game game)
        {
            if (game.HomeTeam != null)
                return game.HomeTeam.League.Name;
            if (game.AwayTeam != null)
                return game.AwayTeam.League.Name;
            if (game.Bracket != null)
                return game.Bracket.League.Name;
            return null;
        }
        public virtual IList<Game> GetGames()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            Game g = null;
            return session.QueryOver<Web.Models.Game>(() => g)
                .JoinQueryOver<Team>(c => c.HomeTeam)
                .Where(m => m.League.Id == this.Id)
                .List();
        }
        public virtual IList<TeamPlayer> GetTeamPlayers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            League l = null;
            Team team = null;
            TeamPlayer tp = null;
            return session.QueryOver<Web.Models.TeamPlayer>(() => tp)
                .JoinQueryOver(() => tp.Team, () => team)
                .JoinQueryOver(() => team.League, () => l)
                .Where(() => l.Id == this.Id).List();

            //return session.QueryOver<Web.Models.League>(() => l).Where(() => l.Id == this.Id)
              //  .JoinAlias(() => l.Teams, () => team).List()
                //.Select(() => );
                //.List<TeamPlayer>();
        }
        public virtual IList<Player> GetPlayers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            Player player = null;
            League l = null;
            Team team = null;
            return session.QueryOver<Web.Models.League>(() => l).Where(() => l.Id == this.Id)
                .JoinAlias(() => l.Teams, () => team)
                .JoinAlias(() => team.Players, () => player)
                .List<Player>();
        }
    }
}