using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NHibernate.Criterion;
using Web.Controllers;
using Web.Helpers;

namespace Web.Models
{
    /// <summary>
    /// A match between two opposing teams.
    /// </summary>
    public class Game
    {
        public virtual int Id { get; set; }
        public virtual Team HomeTeam { get; set; }
        public virtual Team AwayTeam { get; set; }
        public virtual Team TeamToRequestGame { get; set; }
        public virtual Team TeamToConfirmGame { get; set; }
        public virtual DateTime GameDate { get; set; }
        public virtual Location Location { get; set; }
        public virtual int? Innings { get; set; }
        public virtual int? HomeTeamScore { get; set; }
        public virtual int? AwayTeamScore { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual Fee UmpireFee { get; set; }
        public virtual Umpire PlateUmpire { get; set; }
        public virtual Umpire FieldUmpire { get; set; }
        public virtual GameStatus Status { get; set; }
        public virtual String Field { get; set; }
        public virtual Bracket Bracket { get; set; }
        public virtual int? BracketBracket { get; set; }
        public virtual int? BracketPosition { get; set; }
        public virtual Division Division { get; set; }
        public virtual IList<PlayerGameStat> PlayerGameStats { get; set; }

        public virtual String GetLeagueName
        {
            get { return League.LeagueNameFromGame(this); }
        }

        public static IList<Game> GetGamesForTeam(Team team)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .Where(t => t.HomeTeam == team || t.AwayTeam == team)
                .List();
        }

        public static Game GetGameById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Game>(id);
        }

        public static Game GetGameById(int id, User user)
        {
            return GetGamesForUser(user).Where(t => t.Id == id).SingleOrDefault();
        }

        public static bool ScheduleAwayGame(Team homeTeam, Team awayTeam, User requestor, Location location, DateTime date)
        {
            switch (requestor.Role)
            {
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(requestor);
                    if (!homeTeam.Coaches.Contains(coach))
                        return false;
                    break;
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(requestor);
                    if (!homeTeam.Managers.Contains(manager))
                        return false;
                    break;
            }

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                var game = new Game();
                game.AwayTeam = awayTeam;
                game.HomeTeam = homeTeam;
                game.CreatedBy = requestor;
                game.CreatedOn = DateTime.Now;
                game.GameDate = date;
                game.Location = location;
                game.Status = GameStatus.Requested;
                game.TeamToConfirmGame = awayTeam;
                game.TeamToRequestGame = homeTeam;
                game.Division = game.HomeTeam.Division.MaxAge > game.AwayTeam.Division.MaxAge ? game.HomeTeam.Division : game.AwayTeam.Division;
                session.Save(game);
                tx.Commit();
                EmailNotification.GamesRequested(game, requestor, game.HomeTeam, game.AwayTeam);
            }
            //MessagingController.SendMessageToRecipients("Game has been scheduled");
            return true;
        }

        public static bool ScheduleAwayGame(Team homeTeam, Team awayTeam, User requestor, AvailableDates details)
        {
            if (details.Team != awayTeam)
                return false;
            return ScheduleAwayGame(homeTeam, awayTeam, requestor, details.Location, details.Date);
        }

        public static IList<Game> GetGamesForUser(User user)
        {
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return GetAllGames();
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(user);
                    return GetGamesWithManager(manager);
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(user);
                    return GetGamesWithCoach(coach);
                case UserRole.Umpire:
                    var umpire = Umpire.GetUmpireForUser(user);
                    return GetGamesWithUmpire(umpire);
                default:
                    throw new ApplicationException("User not authorized to manage teams.");
            }
        }

        public static IList<Game> GetUpcomingGamesForPlayer(Player player)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var playerTeams = player.Teams.Select(t=>t.Team).Distinct().ToArray();
            var disjunction = new Disjunction()
                .Add(Restrictions.On<Game>(g => g.HomeTeam).IsIn(playerTeams))
                .Add(Restrictions.On<Game>(g => g.AwayTeam).IsIn(playerTeams));
            var conjunction = new Conjunction();
            conjunction.Add(disjunction)
                .Add(Restrictions.Where<Game>(g=>g.Status == GameStatus.Confirmed))
                .Add(Restrictions.Where<Game>(g=>g.GameDate >= DateTime.Now));

            return session.QueryOver<Game>()
                .Where(conjunction)
                .List();
        }

        public static IList<Game> GetAllGames()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Game>().List();
        }

        public virtual BracketGenerator GetBracketGenerator()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.BracketGenerator>()
                .Where(b => b.Bracket == this.Bracket && b.BracketBracket == this.BracketBracket && b.BracketPosition == this.BracketPosition)
                .List().FirstOrDefault();
        }

        public static IList<Game> GetGamesWithManager(Manager manager)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            Team home = null;
            Team away = null;
            Game g = null;
            return session.QueryOver<Game>(() => g)
                .JoinAlias(c => c.HomeTeam.Managers, () => home)
                .JoinAlias(c => c.AwayTeam.Managers, () => away)
                .Where(m => home.Id == manager.Id || away.Id == manager.Id)
                .TransformUsing(NHibernate.Transform.Transformers.DistinctRootEntity)
                .List();
            //.JoinAlias<Manager>(c=>c.Managers, ()=>managers)
            //.WhereRestrictionOn(()=>managers.
            //.Where(team => team.Managers.Contains(manager))
            //.List();
        }

        public static IList<Game> GetGamesWithCoach(Coach coach)
        {
            var home = GetHomeGamesWithCoach(coach);
            var away = GetAwayGamesWithCoach(coach);
            return home.Concat(away).Distinct().ToList();
        }

        public static IList<Game> GetGamesWithUmpire(Umpire umpire)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            if (umpire.League != null)
                return League.GetLeagueById(umpire.League.Id).GetGames();
            else
                return session.QueryOver<Game>()
                    .Where(m => m.PlateUmpire == umpire || m.FieldUmpire == umpire)
                    .List();
        }

        public static IList<Game> GetHomeGamesWithCoach(Coach coach)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .JoinQueryOver<Team>(c => c.HomeTeam)
                .JoinQueryOver<Coach>(c => c.Coaches)
                .Where(m => m.Id == coach.Id)
                .List();
        }

        public static IList<Game> GetAwayGamesWithCoach(Coach coach)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .JoinQueryOver<Team>(c => c.AwayTeam)
                .JoinQueryOver<Coach>(c => c.Coaches)
                .Where(m => m.Id == coach.Id)
                .List();
        }


        public static Game GetGameByBracket(Bracket bracket, int bracketNum, int positionNum)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .Where(g=>g.Bracket == bracket && g.BracketBracket == bracketNum && g.BracketPosition == positionNum)
                .SingleOrDefault();
        }

        public static IList<Game> GetGamesForBracket(Bracket bracket)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .Where(g => g.Bracket == bracket)
                .List();
        }

        public static IList<Game> GetPoolGamesForBracket(Bracket bracket)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .Where(g => g.Bracket == bracket && g.BracketBracket == null)
                .List();
        }

        public static IList<Game> GetBracketGamesForBracket(Bracket bracket)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Game>()
                .Where(g => g.Bracket == bracket && g.BracketBracket != null)
                .List();
        }

        public static IList<Game> GetCompletedBracketGamesForBracket(Bracket bracket)
        {
            return GetBracketGamesForBracket(bracket).Where(g => g.Status == GameStatus.Played).ToList();
        }
        public static IList<Game> GetUncompletedBracketGamesForBracket(Bracket bracket)
        {
            return GetBracketGamesForBracket(bracket).Where(g => g.Status != GameStatus.Played).ToList();
        }

        public static IList<Game> GetCompletedPoolGamesForBracket(Bracket bracket)
        {
            return GetPoolGamesForBracket(bracket).Where(g => g.Status == GameStatus.Played).ToList();
        }
        public static IList<Game> GetUncompletedPoolGamesForBracket(Bracket bracket)
        {
            return GetPoolGamesForBracket(bracket).Where(g => g.Status != GameStatus.Played).ToList();
        }
        public static bool HavePoolGamesBeenSubmittedToBracket(Bracket bracket)
        {
            return BracketTeam.GetBracketTeams(bracket).Where(g => g.Team == null).ToList().Count > 0 ? false : true;
        }

        public static GameInfoModel MapGameToGameInfo(Game game)
        {
            var gameInfo = new GameInfoModel();
            var bracketGenerator = game.GetBracketGenerator();
            gameInfo.Game = game;
            gameInfo.HomeTeamName = Bracket.TeamNameFromBracket(bracketGenerator, game.HomeTeam, game, true);
            gameInfo.AwayTeamName = Bracket.TeamNameFromBracket(bracketGenerator, game.AwayTeam, game, false);
            gameInfo.HomeTeam = Bracket.TeamFromBracket(bracketGenerator, game.HomeTeam, game, true);
            gameInfo.AwayTeam = Bracket.TeamFromBracket(bracketGenerator, game.AwayTeam, game, false);
            if (game.Bracket != null)
            {
                gameInfo.Type = string.Format("{0} {1} - {2}", (game.BracketBracket != null) ? "Bracket" : "Pool", game.Bracket.Name, game.Bracket.Division.Name);
            }
            else
            {
                gameInfo.Type = "Normal";
            }
            return gameInfo;
        }
    }

    public enum GameStatus
    {
        Requested = 0,
        Confirmed = 1,
        Played = 2,
        Cancelled = 3,
        [Description("Home Forfeit")]
        HomeForfeit = 4,
        [Description("Away Forfeit")]
        AwayForfeit = 5,
        Declined = 99
    }
    public enum GameStatusForUmpires
    {
        Played = 2,
        Cancelled = 3,
        [Description("Home Forfeit")]
        HomeForfeit = 4,
        [Description("Away Forfeit")]
        AwayForfeit = 5,
    }
}