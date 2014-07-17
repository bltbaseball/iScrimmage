using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoddleReport.Writers;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();


        public DoddleReport.Web.ReportResult ExportContacts()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if(user.Role != UserRole.Administrator)
                return null;
            var coaches = Web.Models.Coach.GetAllCoaches();
            var players = Web.Models.Player.GetAllPlayers();
            var guardians = Web.Models.Guardian.GetAllGuardians();
            var managers = Web.Models.Manager.GetAllManagers();
            var umpires = Web.Models.Umpire.GetAllUmpires();
            var users = Web.Models.User.GetAllUsers();

            var emails = (from c in coaches where !string.IsNullOrEmpty(c.Email) select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "Coach" }).ToList();
            emails.AddRange((from c in players where !string.IsNullOrEmpty(c.Email) select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "Player" }).ToList());
            emails.AddRange((from c in guardians where !string.IsNullOrEmpty(c.Email) select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "Guardian" }).ToList());
            emails.AddRange((from c in managers where !string.IsNullOrEmpty(c.Email) select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "Manager" }).ToList());
            emails.AddRange((from c in umpires where !string.IsNullOrEmpty(c.Email) select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "Umpires" }).ToList());
            //emails.AddRange((from c in users select new ContactModel { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, Type = "User" }).ToList());

            var report = new DoddleReport.Report(new DoddleReport.ReportSources.EnumerableReportSource(emails));
            report.RenderHints[DelimitedTextReportWriter.DelimiterHint] = DelimitedTextReportWriter.CommaDelimiter;
            var result = new DoddleReport.Web.ReportResult(report);
            result.FileName = string.Format("EmailContacts");
            return result;
        }

        //
        // GET: /Profile/
        public ActionResult Index()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            switch (user.Role)
            {
                case UserRole.Coach:
                    return RedirectToAction("Coach", "Profile");
                case UserRole.Manager:
                    return RedirectToAction("Manager", "Profile");
                case UserRole.Player:
                    return RedirectToAction("Player", "Profile");
                case UserRole.Umpire:
                    return RedirectToAction("Umpire", "Profile");
                case UserRole.Guardian:
                    return RedirectToAction("Guardian", "Profile");
                default:
                    return RedirectToAction("Dashboard", "Home");
            }
        }

        public ActionResult Player()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var player = Web.Models.Player.GetPlayerForUser(user);
            if (player == null)
                return RedirectToAction("Index");

            var model = new PlayerProfileModel();
            model.Player = player;
            model.TeamsActive = player.Teams.Where(t => t.Team.IsActive == true).ToList();
            model.Games = Web.Models.Game.GetUpcomingGamesForPlayer(player);
            return View(model);
        }

        public ActionResult Coach()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var coach = Web.Models.Coach.GetCoachForUser(user);
            var model = new CoachProfileModel();
            model.Coach = coach;
            var activeTeams = coach.Teams.Where(team => team.IsActive == true).ToList();
            model.TeamsActive = TeamDTO.MapTeamsToModel(activeTeams);
            model.TeamsInactive = TeamDTO.MapTeamsToModel(coach.Teams.Where(team => team.IsActive == false).ToList());
            var games = Web.Models.Game.GetGamesWithCoach(coach).OrderByDescending(game => game.GameDate).ToList();
            model.PastGames = GameDTO.MapGamesToModel(games.Where(game => game.GameDate <= DateTime.Now).ToList());
            model.UpcomingGames = GameDTO.MapGamesToModel(games.Where(game => game.GameDate > DateTime.Now).ToList());
            model.UpcomingGames = model.UpcomingGames.Concat(GameDTO.MapGamesToModel(games.Where(game => game.GameDate <= DateTime.Now && (game.Status == GameStatus.Confirmed)).ToList())).ToList();
            model.UpcomingGames = model.UpcomingGames.OrderByDescending(game => game.GameTime).ToList();
            var dates = Web.Models.AvailableDates.GetOpenDatesForTeam(activeTeams);
            model.AvailableDates = AvailableDatesDTO.MapAvailableDates(dates);
            return View(model);
        }        

        public ActionResult Manager()
        {
            return View();
        }

        public ActionResult Umpire()
        {
            return View();
        }

        public ActionResult Guardian()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var guardian = Web.Models.Guardian.GetGuardianForUser(user);
            var players = Web.Models.Player.GetPlayersForUser(user);
            var model = new GuardianProfileModel();
            model.Guardian = guardian;
            model.Players = new List<PlayerProfileModel>();
            foreach (var player in players)
            {
                var playerModel = new PlayerProfileModel();
                playerModel.Player = player;
                playerModel.TeamsActive = player.Teams.Where(t => t.Team.IsActive == true).ToList();
                playerModel.TeamsInactive = player.Teams.Where(t => t.Team.IsActive == false).ToList();
                playerModel.Games = Web.Models.Game.GetUpcomingGamesForPlayer(player);
                model.Players.Add(playerModel);                
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestWaiver(int playerId, int teamId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var players = Web.Models.Player.GetPlayersForUser(user);
            var player = players.Where(i => i.Id == playerId).SingleOrDefault();
            if (player != null)
            {
                var teamPlayer = player.Teams.Where(t => t.Team.Id == teamId).SingleOrDefault();
                if (teamPlayer != null)
                {
                    var RequestId = Hellosign.RequestPlayerWaiverSignature(session, teamPlayer, player, teamPlayer.Team);
                    
                    TempData["WaiverRequested"] = true;
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

    }
}
