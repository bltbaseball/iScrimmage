using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Criterion;
using Web.Models;
using Web.Helpers;

namespace Web.Models
{
    public class AvailableDates
    {
        public virtual int Id { get; set; }
        public virtual Team Team { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Location Location { get; set; }
        public virtual bool IsHome { get; set; }
        public virtual bool IsAway { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// For IsAway dates, DistanceFromLocation is the distance, in miles, from the Location the Team is willing to travel for the game. Opposing Team's Location must fall within this distance.
        /// </summary>
        public virtual int? DistanceFromLocation { get; set; }

        /// <summary>
        /// This property should probably stay unused. GameScheduled should instead be a dynamic lookup of games for this team that matches this DateTime.
        /// </summary>
        public virtual bool GameScheduled { get; set; }

        /// <summary>
        /// Gets available game dates opposing Teams have marked that match the criteria.
        /// </summary>
        /// <param name="team">The Team looking for available game dates.</param>
        /// <param name="division">The opposing Team's division.</param>
        /// <param name="miles">The distance, in miles, the game Location must be to the Team's Location for Home games.</param>
        /// <returns></returns>
        public static IList<AvailableDateResult> GetAvailableDatesForTeamToPick(Team team, Division division, int miles, Location location)
        {
            if (location == null)
                return new List<AvailableDateResult>();

            // by default, division is the Team's division
            // location is Team.Location
            // miles = 50
            var dateDivision = team.Division;
            if (division != null)
                dateDivision = division;

            // get available dates for opposing team's that are date.IsHome within miles distance of date.Location from team.Location

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            //AvailableDates date = null;
            //Team dt = null;
            var homeDates = session.QueryOver<AvailableDates>()
                //.JoinAlias(() => date.Team, () => dt)
                .Where(date => date.Team != team
                    && date.Date > DateTime.Now
                    && date.IsHome
                )
                .JoinQueryOver<Team>(t => t.Team)
                .Where(t => t.Division == dateDivision && t.League == team.League)
                .List<AvailableDates>();

            //var crit = DetachedCriteria.For(typeof(AvailableDates));
            //crit.CreateAlias("Team", "t");
            ////crit.SetProjection(SpatialProjections.Distance<AvailableDates>(d => d.Location.Point, team.Location.Point));
            //crit.Add(Restrictions.Not(Restrictions.Eq("Team", team)));
            //crit.Add(Restrictions.Gt("Date", DateTime.Now));
            //crit.Add(Restrictions.Eq("IsHome", true));
            //crit.Add(Restrictions.Eq("t.Division", dateDivision));
            //crit.Add(Restrictions.Eq("t.League", team.League));
            //crit.Add(SpatialRestrictions.IsWithinDistance("Location.Point", team.Location.Point, miles * 1609.32));
            ////crit.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<AvailableDateResult>());

            // SQL Server to the Rescue (how infrequently has that phrase been written?)
            var matchedLocation = Microsoft.SqlServer.Types.SqlGeography.Point(location.Longitude, location.Latitude, 4326);
            var matchingOpposingHomeGames = from d in homeDates
                                            where (double)Microsoft.SqlServer.Types.SqlGeography.Point(d.Location.Longitude, d.Location.Latitude, 4326)
                                                .STDistance(matchedLocation) <= (miles * 1609.34)
                                            select new AvailableDateResult
                                            {
                                                AvailableDate = d,
                                                Distance = (double)Microsoft.SqlServer.Types.SqlGeography.Point(d.Location.Longitude, d.Location.Latitude, 4326)
                                                .STDistance(matchedLocation)
                                            };

            var awayDates = session.QueryOver<AvailableDates>()
                //.JoinAlias(() => date.Team, () => dt)
                .Where(date => date.Team != team
                   && date.Date > DateTime.Now
                   && date.IsAway
                   && date.DistanceFromLocation != null)
                   .JoinQueryOver(t => t.Team)
                   .Where(t => t.Division == dateDivision && t.League == team.League)
               .List<AvailableDates>();
            var matchingOpposingAwayGames = from d in awayDates
                                            where
                                            (double)Microsoft.SqlServer.Types.SqlGeography.Point(d.Location.Longitude, d.Location.Latitude, 4326)
                                                .STDistance(matchedLocation) <= (d.DistanceFromLocation.Value * 1609.34)
                                            select new AvailableDateResult
                                            {
                                                AvailableDate = d,
                                                Distance = (double)Microsoft.SqlServer.Types.SqlGeography.Point(d.Location.Longitude, d.Location.Latitude, 4326)
                                                .STDistance(matchedLocation)
                                            };

            var matchingDates = new List<AvailableDateResult>();
            matchingDates.AddRange(matchingOpposingHomeGames.ToList());
            matchingDates.AddRange(matchingOpposingAwayGames.ToList());
            matchingDates = matchingDates.Distinct().ToList();
            // remove entries with the same Ids
            var distinctDates = matchingDates
                .GroupBy(d => d.AvailableDate.Id)
                .Select(d => d.First())
                .ToList();



            // filter out dates that have a game scheduled
            //matchingDates = matchingDates.Where(d => d.GetGameScheduled() == null).ToList(); // this feels wrong
            distinctDates = distinctDates.Where(d => d.AvailableDate.GetGameScheduled() == null && d.AvailableDate.Team.IsActive).Distinct().ToList(); // still feels wrong
            return distinctDates;
        }

        public static Game GetGameScheduled(AvailableDates date)
        {
            var games = Game.GetGamesForTeam(date.Team);
            var available = new List<AvailableDates>();
            var scheduledGame = (from g in games where g.GameDate == date.Date select g).FirstOrDefault();
            if (scheduledGame == null)
                return null;
            return scheduledGame;
        }

        /// <summary>
        /// Um, this is basically a read-only property, so...
        /// </summary>
        /// <returns>Feels bad, man.</returns>
        public virtual Game GetGameScheduled()
        {
            // get the list of dates available for a team -- has availabledate during which a game is not scheduled
            var games = Game.GetGamesForTeam(Team);
            var available = new List<AvailableDates>();
            var scheduledGame = (from g in games where g.GameDate == Date && (g.Status == GameStatus.Confirmed || g.Status == GameStatus.Played) select g).FirstOrDefault();
            if (scheduledGame == null)
                return null;
            return scheduledGame;
        }

        public static IList<AvailableDates> GetAvailableDatesForTeam(Team team)
        {
            // get the list of dates available for a team -- has availabledate during which a game is not scheduled
            var games = Game.GetGamesForTeam(team);
            var dates = team.DatesAvailable;
            var available = new List<AvailableDates>();
            foreach (var date in dates)
            {
                // is a game scheduled within a 2 hour window of this availabilty?
                var scheduledGame = (from g in games where g.GameDate >= date.Date.AddHours(-1) && g.GameDate <= date.Date.AddHours(1) select g).FirstOrDefault();
                if (scheduledGame == null)
                {
                    available.Add(date);
                }
            }
            return available;
        }

        public static AvailableDates GetAvailableDateById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<AvailableDates>(id);
        }

        public static IList<AvailableDates> GetOpenDatesForTeam(IList<Team> teams)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<AvailableDates>()
                .Where(t => t.Team.Id != 1 && t.Date > DateTime.Now) /*(teams.ToList().Contains(t.Team) == false)*/
                .List();
        }

    }

    public class Team
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }
        public virtual League League { get; set; }
        public virtual Division Division { get; set; }
        public virtual TeamClass Class { get; set; }
        public virtual IList<TeamPlayer> Players { get; set; }
        public virtual IList<Manager> Managers { get; set; }
        public virtual IList<Coach> Coaches { get; set; }
        public virtual IList<AvailableDates> DatesAvailable { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual Location Location { get; set; }
        public virtual bool IsLookingForPlayers { get; set; }
        public virtual string HtmlDescription { get; set; }

        //public virtual IList<FeePayment> FeePayments { get; set; }
        //public virtual IList<Fee> Fees { get; set; }

        public virtual Team PriorTeam { get; set; }

        public virtual bool IsActive
        {
            get
            {
                if (!this.League.IsActive)
                    return false;
                if (this.League.EndDate < DateTime.Now)
                    return false;
                return true;
            }
        }

        public static string PrettyName(Team team)
        {
            return team.Name + " " + team.Division.Name + " " + team.Class.Name + " " + team.League.Name;
        }

        public static string PrettyName(Team team, Game game)
        {
            if (team == null)
                return null;
            if (game != null)
            {
                if (game.Status == Web.Models.GameStatus.Played &&
                    (game.HomeTeamScore > game.AwayTeamScore && team == game.HomeTeam ||
                    game.HomeTeamScore < game.AwayTeamScore && team == game.AwayTeam))
                {
                    return "<a href='/Overview/Team/" + team.Id + "'><b>" + team.Name + " " + team.Division.Name + " " + team.Class.Name + " " + team.League.Name + "</b></a>";
                }
            }
            return "<a href='/Overview/Team/" + team.Id + "'>" + team.Name + " " + team.Division.Name + " " + team.Class.Name + " " + team.League.Name + "</a>";
        }

        public static string PrettyNameWithoutLeague(Team team)
        {
            return team.Name + " " + team.Division.Name + " " + team.Class.Name;
        }

        public static string PrettyNameWithoutLeague(Team team, Game game)
        {
            if (team == null)
                return null;
            if (game != null)
            {
                if (game.Status == Web.Models.GameStatus.Played &&
                    (game.HomeTeamScore > game.AwayTeamScore && team == game.HomeTeam ||
                    game.HomeTeamScore < game.AwayTeamScore && team == game.AwayTeam))
                {
                    return "<a href='/Overview/Team/" + team.Id + "'><b>" + team.Name + /*" " + team.Division.Name + " " + team.Class.Name +*/ "</b></a>";
                }
            }
            return  "<a href='/Overview/Team/" + team.Id + "'>" + team.Name + /*" " + team.Division.Name + " " + team.Class.Name +*/ "</a>";
        }

        public static IList<Team> GetActiveTeamsLookingForPlayers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Team>()
                .Where(t => t.IsLookingForPlayers == true)
                .List().Where(t => t.IsActive == true).OrderBy(t => t.Name).ToList();
        }

        public static IList<Team> GetActiveTeamsLookingForPlayers(Player player)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            if (player.DateOfBirth.HasValue)
                return session.QueryOver<Web.Models.Team>()
                    .Where(t => t.IsLookingForPlayers == true)
                    .List().Where(t => t.IsActive == true && (PlayerHelper.PlayerAge(player.DateOfBirth.Value) == t.Division.MaxAge || PlayerHelper.PlayerAge(player.DateOfBirth.Value) + 1 == t.Division.MaxAge)).OrderBy(t => t.Name).ToList();
            else
                return GetActiveTeamsLookingForPlayers();
        }

        public static IList<Team> GetAllTeams()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Team>().List();
        }

        public static bool ValidateTeamForUser(Team team, User user)
        {
            var teams = GetTeamsForUser(user);
            if (teams.Contains(team))
                return true;
            return false;
        }

        public static Team CopyTeam(Team team, League league, User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var teamToCopy = Web.Models.Team.GetTeamById(team.Id, user);
            if (teamToCopy == null)
                return null;

            bool copyPhotos = false;
            if (league.Type == LeagueType.Tournament)
                copyPhotos = true;

            using (var tx = session.BeginTransaction())
            {
                var item = new Team();
                item.Class = team.Class;
                item.Coaches = new List<Coach>();
                foreach (var coach in team.Coaches)
                {
                    item.Coaches.Add(coach);
                }
                item.CreatedOn = DateTime.Now;
                item.Division = team.Division;
                item.DatesAvailable = new List<AvailableDates>();
                item.League = league;
                item.Managers = new List<Manager>();
                foreach (var manager in team.Managers)
                {
                    item.Managers.Add(manager);
                }

                item.Name = team.Name;
                item.Players = new List<TeamPlayer>();
                item.PriorTeam = team;
                item.Url = team.Url;
                item.Location = team.Location;
                item.HtmlDescription = team.HtmlDescription;
                session.Save(item);
                foreach (var player in team.Players)
                {
                    var itemPlayer = new TeamPlayer();
                    itemPlayer.Team = item;
                    itemPlayer.Player = player.Player;
                    itemPlayer.CreatedOn = DateTime.Now;
                    if (copyPhotos)
                    {
                        itemPlayer.IsPhotoVerified = player.IsPhotoVerified;
                        itemPlayer.Photo = player.Photo;
                        itemPlayer.PhotoVerifiedBy = player.PhotoVerifiedBy;
                    }
                    itemPlayer.JerseyNumber = player.JerseyNumber;
                    itemPlayer.Status = player.Status;
                    itemPlayer.WaiverStatus = SignStatus.NotSigned;
                    session.Save(itemPlayer);
                    item.Players.Add(itemPlayer);
                }
                session.Update(item);
                tx.Commit();
                return item;
            }
        }

        public static TeamStatusModel GetTeamStatusForTeam(Team team)
        {
            var model = new TeamStatusModel();
            model.Team = team;
            model.HasAvailableDatesEntered = true;
            if (team.League.Type == LeagueType.League)
            {
                if (team.League.MinimumDatesAvailable.HasValue)
                {
                    if (team.DatesAvailable.Count < team.League.MinimumDatesAvailable.Value)
                        model.HasAvailableDatesEntered = false;
                }
            }

            model.HasMinimumNumberOfPlayers = (team.Players.Count >= 9);
            model.HasPaidMandatoryFees = FeePayment.GetFeesPaidStatus(team);
            model.HasPhotosSubmitted = (team.Players.Count == 0) ? false : true;
            foreach (var player in team.Players)
            {
                if (string.IsNullOrEmpty(player.Photo))
                {
                    model.HasPhotosSubmitted = false;
                    break;
                }
            }

            model.HasValidBirthdates = (team.Players.Count == 0) ? false : true;
            foreach (var player in team.Players)
            {
                if (!player.Player.DateOfBirth.HasValue)
                {
                    model.HasValidBirthdates = false;
                    break;
                }

                var cutOffDate = Web.Helpers.PlayerHelper.PlayerCutoffDate(team.Division.MaxAge);
                if (player.Player.DateOfBirth <= cutOffDate)
                {
                    model.HasValidBirthdates = false;
                    break;
                }
            }

            model.HasWaiversSigned = (team.Players.Count == 0) ? false : true;
            foreach (var player in team.Players)
            {
                if (player.WaiverStatus != SignStatus.Signed)
                {
                    model.HasWaiversSigned = false;
                    break;
                }
            }
            return model;
        }

        public static IList<Team> GetTeamsForUser(User user)
        {
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return GetAllTeams();
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(user);
                    return GetTeamsWithManager(manager);
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(user);
                    return GetTeamsWithCoach(coach);
                case UserRole.Umpire:
                    return GetAllTeams();
                default:
                    return null;
                //throw new ApplicationException("User not authorized to manage teams.");
            }
        }

        public static Team GetTeamById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Team>(id);
        }

        public static Team GetTeamById(int id, User user)
        {
            return GetTeamsForUser(user).Where(t => t.Id == id).SingleOrDefault();
        }

        public static IList<Team> GetTeamsWithManager(Manager manager)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Team>()
                .JoinQueryOver<Manager>(c => c.Managers)
                .Where(m => m.Id == manager.Id)
                .List();
            //.JoinAlias<Manager>(c=>c.Managers, ()=>managers)
            //.WhereRestrictionOn(()=>managers.
            //.Where(team => team.Managers.Contains(manager))
            //.List();
        }

        public static IList<Team> GetTeamsWithCoach(Coach coach)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Team>()
                .JoinQueryOver<Coach>(c => c.Coaches)
                .Where(m => m.Id == coach.Id)
                .List();
        }

        public virtual int Wins
        {
            get
            {
                var teamGames = Models.Game.GetGamesForTeam(this);
                return teamGames.Where(g => (((g.HomeTeam == this) && (g.HomeTeamScore > g.AwayTeamScore)) || ((g.AwayTeam == this) && (g.AwayTeamScore > g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
            }
        }
        public virtual int Losses
        {
            get
            {
                var teamGames = Models.Game.GetGamesForTeam(this);
                return teamGames.Where(g => (((g.HomeTeam == this) && (g.HomeTeamScore < g.AwayTeamScore)) || ((g.AwayTeam == this) && (g.AwayTeamScore < g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
            }
        }
        public virtual int Ties
        {
            get
            {
                var teamGames = Models.Game.GetGamesForTeam(this);
                return teamGames.Where(g => (((g.HomeTeam == this) && (g.HomeTeamScore == g.AwayTeamScore)) || ((g.AwayTeam == this) && (g.AwayTeamScore == g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
            }
        }
        public virtual int TotalGames
        {
            get
            {
                var teamGames = Models.Game.GetGamesForTeam(this);
                return teamGames.Count;
            }
        }

    }

    /// <summary>
    /// Skill level of play for a team. (A, AA, AAA, Majors). Shared across leagues and teams. Two teams matched up with different classes will be assessed a handicap. 
    /// </summary>
    public class TeamClass
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Handicap { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        //public virtual List<Division> Divisions { get; set; }
    }
}