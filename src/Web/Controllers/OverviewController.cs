using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class OverviewController : Controller
    {
        //
        // GET: /Overview/

        public ActionResult Index()
        {
            var model = new LeaguesOverviewModel();
            var leagueOverviews = new List<LeagueOverviewModel>();
            var leagues = Web.Models.League.GetAllLeagues();
            foreach (var league in leagues)
            {
                leagueOverviews.Add(CreateOverviewForLeague(league));
            }

            model.All = leagueOverviews.OrderByDescending(l => l.League.IsActive).ThenByDescending(l => l.League.EndDate).ToList();
            model.Leagues = leagueOverviews.Where(l => l.League.Type == LeagueType.League).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();
            model.Tournaments = leagueOverviews.Where(l => l.League.Type == LeagueType.Tournament).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();
            model.Scrimmages = leagueOverviews.Where(l => l.League.Type == LeagueType.Scrimmage).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();

            return View(model);
        }

        public ActionResult Scrimmage()
        {
            return Index();
        }

        public ActionResult League()
        {
            return Index();
        }

        public ActionResult Tournament()
        {
            return Index();
        }

        public ActionResult Other()
        {
            var model = new LeaguesOverviewModel();
            var leagueOverviews = new List<LeagueOverviewModel>();
            var leagues = Web.Models.League.GetAllOther();
            foreach (var league in leagues)
            {
                leagueOverviews.Add(CreateOverviewForLeague(league));
            }

            model.All = leagueOverviews.OrderByDescending(l => l.League.IsActive).ThenByDescending(l => l.League.EndDate).ToList();
            model.Leagues = leagueOverviews.Where(l => l.League.Type == LeagueType.League).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();
            model.Tournaments = leagueOverviews.Where(l => l.League.Type == LeagueType.Tournament).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();
            model.Scrimmages = leagueOverviews.Where(l => l.League.Type == LeagueType.Scrimmage).OrderByDescending(l => l.League.IsActive).OrderByDescending(l => l.League.EndDate).ToList();

            return View(model);
        }

        private LeagueOverviewModel CreateOverviewForLeague(League league)
        {
            var model = new LeagueOverviewModel();
            model.League = league;
            model.Name = league.Name;
            model.EndDate = league.EndDate.ToString();
            var games = new List<Game>();
            var rankings = new List<TeamRankingModel>();
            foreach (var team in model.League.Teams)
            {
                var teamGames = Game.GetGamesForTeam(team);
                games.AddRange(teamGames);

                // todo: include homeforfeit and awayforfeit in win/loss calculation
                var ranking = new TeamRankingModel();
                ranking.Team = team;
                ranking.Wins = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore > g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore > g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Losses = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore < g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore < g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Ties = teamGames.Where(g => (((g.HomeTeam == team) && (g.HomeTeamScore == g.AwayTeamScore)) || ((g.AwayTeam == team) && (g.AwayTeamScore == g.HomeTeamScore))) && g.Status == GameStatus.Played && g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue).Count();
                ranking.Games = teamGames.Count;

                var totalRunsScored = new List<int>();
                totalRunsScored.AddRange(teamGames.Where(g => g.HomeTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.HomeTeamScore.Value));
                totalRunsScored.AddRange(teamGames.Where(g => g.AwayTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.AwayTeamScore.Value));
                ranking.RunsScored = totalRunsScored.Sum();

                var totalRunsAllowed = new List<int>();
                totalRunsAllowed.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.AwayTeamScore.Value));
                totalRunsAllowed.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => g.HomeTeamScore.Value));
                ranking.RunsAllowed = totalRunsAllowed.Sum();

                var totalRunsDifferential = new List<int>();
                totalRunsDifferential.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.AwayTeamScore.Value - g.HomeTeamScore.Value)));
                totalRunsDifferential.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.HomeTeamScore.Value - g.AwayTeamScore.Value)));
                ranking.RunDifferential = totalRunsDifferential.Sum();

                var totalRunsDifferentialMax8 = new List<int>();
                totalRunsDifferentialMax8.AddRange(teamGames.Where(g => g.HomeTeam == team && g.AwayTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.AwayTeamScore.Value - g.HomeTeamScore.Value) > 8 ? 8 : (g.AwayTeamScore.Value - g.HomeTeamScore.Value)));
                totalRunsDifferentialMax8.AddRange(teamGames.Where(g => g.AwayTeam == team && g.HomeTeamScore.HasValue && g.Status == GameStatus.Played).Select(g => (g.HomeTeamScore.Value - g.AwayTeamScore.Value) > 8 ? 8 : (g.HomeTeamScore.Value - g.AwayTeamScore.Value)));
                ranking.RunDifferentialMax8 = totalRunsDifferentialMax8.Sum();
                rankings.Add(ranking);
            }

            int i = 1;
            foreach (var rank in (from r in rankings orderby r.Wins descending, r.Losses descending, r.RunsScored descending, r.RunsAllowed descending select r))
            {
                rank.Ranking = i++;
            }
            model.Rankings = (from r in rankings orderby r.Ranking select r).ToList();

            var bracketOverviews = new List<BracketOverviewModel>();
            var brackets = Web.Models.Bracket.GetBracketsForLeague(model.League);
            foreach (var bracket in brackets)
            {
                bracketOverviews.Add(Web.Models.Bracket.PopulateBracketOverview(bracket));
                games.AddRange(Game.GetGamesForBracket(bracket));
            }

            model.Brackets = bracketOverviews;

            model.Games = games.Distinct().OrderBy(g => g.GameDate).ToList();

            return model;
        }

        
        public ActionResult Details(int id)
        {
            var league = Web.Models.League.GetLeagueById(id);
            if (league == null)
                return RedirectToAction("Index");
            var model = CreateOverviewForLeague(league);
            return View(model);
        }

        public ActionResult Team(int id)
        {
            var team = Web.Models.Team.GetTeamById(id);
            if (team == null)
                return RedirectToAction("Index");

            var model = new TeamOverviewModel();
            model.Team = team;
            model.Players = TeamPlayerDTO.MapTeamPlayers(team.Players.OrderBy(p => p.Player.LastName).ToList());
            model.Games = GameDTO.MapGamesToModel(Game.GetGamesForTeam(team).OrderByDescending(g => g.GameDate).ToList());
            return View(model);
        }

        public ActionResult Pitch(int id)
        {
            var league = Web.Models.League.GetLeagueById(id);
            if (league == null)
                return RedirectToAction("Index");

            var model = new PitchOverviewModel();
            model.League = league;
            model.Players = TeamPlayerDTO.MapTeamPlayers(league.GetTeamPlayers().Where(p => p.PlayerGameStats.Count > 0)
                .Where(p => p.PlayerGameStats.Any(s => s.InningsPitched.HasValue || s.PitchesThrown.HasValue || s.InningsOuts.HasValue)).OrderBy(p => p.Player.LastName).ToList());
            return View(model);
        }

        public ActionResult LeaguePlayers(int id)
        {
            var league = Web.Models.League.GetLeagueById(id);
            if (league == null)
                return RedirectToAction("Index");

            var model = new TeamOverviewModel();
            model.League = league;
            model.Players = TeamPlayerDTO.MapTeamPlayers(league.GetTeamPlayers().Where(p => p.PlayerGameStats.Count > 0).ToList());
            return View(model);
        }

        public ActionResult LeagueCoaches(int id)
        {
            var league = Web.Models.League.GetLeagueById(id);
            if (league == null)
                return RedirectToAction("Index");

            var model = new CoachOverviewModel();
            model.League = league;
            model.Coaches = TeamCoachDTO.MapTeamCoaches(league.Teams).OrderBy(t => t.Team.Name).ToList();
            return View(model);
        }

        public ActionResult LeagueTeams(int id)
        {
            var league = Web.Models.League.GetLeagueById(id);
            if (league == null)
                return RedirectToAction("Index");

            var model = new LeagueOverviewModel();
            model.League = league;
            return View(model);
        }

        public ActionResult Umpire(int id)
        {
            var item = Web.Models.Umpire.GetUmpireById(id);
            if (item == null)
                return RedirectToAction("Index");

            var model = new UmpireOverviewModel();
            model.Umpire = item;
            model.Games = Web.Models.Game.GetGamesWithUmpire(item).OrderByDescending(x => x.GameDate).ToList();
            return View(model);
        }

        public ActionResult Location(int id)
        {
            var item = Web.Models.Location.GetLocationById(id);
            if (item == null)
                return RedirectToAction("Index");

            var model = new LocationOverviewModel();
            model.Name = item.Name;
            model.Address = item.Address;
            model.City = item.City;
            model.State = item.State;
            model.Zip = item.Zip;
            model.Url = item.Url;
            model.Notes = item.Notes;
            model.GroundsKeeperPhone = item.GroundsKeeperPhone;

            return View(model);
        }

        public ActionResult Bracket(int id)
        {
            var item = Web.Models.Bracket.GetBracketById(id);
            if (item == null)
                return RedirectToAction("Index");

            var model = Web.Models.Bracket.PopulateBracketOverview(item);
            return View(model);
        }

    }
}
