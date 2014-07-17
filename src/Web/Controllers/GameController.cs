using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager, UserRole.Umpire)]
    public class GameController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        private IList<SelectListItem> GetTeamsList()
        {
            var items = new List<SelectListItem>();
            var teams = Team.GetAllTeams().OrderByDescending(l => l.Name);
            foreach (var item in teams)
            {
                items.Add(new SelectListItem { Text = Team.PrettyName(item), Value = item.Id.ToString() });
            }
            return items;
        }

        private IList<SelectListItem> GetTeamsList(int leagueId)
        {
            var items = new List<SelectListItem>();
            var teams = Team.GetAllTeams().Where(t => t.League.Id == leagueId).OrderByDescending(l => l.Name);
            foreach (var item in teams)
            {
                items.Add(new SelectListItem { Text = Team.PrettyNameWithoutLeague(item), Value = item.Id.ToString() });
            }
            return items;
        }

        private IList<SelectListItem> GetDistancesList()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value="5", Text = "5 miles" });
            items.Add(new SelectListItem() { Value = "10", Text = "15 miles" });
            items.Add(new SelectListItem() { Value = "15", Text = "15 miles" });
            items.Add(new SelectListItem() { Value = "20", Text = "20 miles" });
            items.Add(new SelectListItem() { Value = "25", Text = "25 miles" });
            items.Add(new SelectListItem() { Value = "50", Text = "50 miles", Selected = true });
            items.Add(new SelectListItem() { Value = "100", Text = "100 miles" });
            return items;
        }

        private IList<SelectListItem> GetDivisionList()
        {
            var items = new List<SelectListItem>();
            var divisions = Division.GetAllDivisions();
            foreach (var item in divisions)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        private IList<SelectListItem> GetInningsList()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "7", Value = "7" });
            items.Add(new SelectListItem { Text = "9", Value = "9" });
            return items;
        }

        public IList<SelectListItem> GetLeagueList()
        {
            var items = new List<SelectListItem>();
            var leagues = session.QueryOver<League>().OrderBy(l => l.CreatedOn).Desc.List();
            foreach (var item in leagues)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        const string FilterOptions_NoFieldUmpire = "No Field Umpire";
        const string FilterOptions_NoPlateUmpire = "No Plate Umpire";
        const string FilterOptions_NoUmpires = "No Umpires";

        private IList<SelectListItem> GetFilterOptions()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "All Games", Value = "" });
            items.Add(new SelectListItem() { Text = FilterOptions_NoFieldUmpire, Value = FilterOptions_NoFieldUmpire });
            items.Add(new SelectListItem() { Text = FilterOptions_NoPlateUmpire, Value = FilterOptions_NoPlateUmpire });
            items.Add(new SelectListItem() { Text = FilterOptions_NoUmpires, Value = FilterOptions_NoUmpires });
            return items;
        }

        public IList<SelectListItem> CreateListFromTeams(IList<Team> list)
        {
            var items = new List<SelectListItem>();
            foreach (var item in list)
            {
                items.Add(new SelectListItem { Text = Team.PrettyNameWithoutLeague(item), Value = item.Id.ToString() });
            }
            return items;
        }


        public IList<SelectListItem> GetMyTeamsList(User user)
        {
            var items = new List<SelectListItem>();
            var results = Team.GetTeamsForUser(user);
            foreach (var item in results)
            {
                items.Add(new SelectListItem { Text = Team.PrettyName(item), Value = item.Id.ToString() });
            }
            return items;
        }

        public IList<SelectListItem> GetMyTeamsList(User user, int leagueId)
        {
            var items = new List<SelectListItem>();
            var results = Team.GetTeamsForUser(user).Where(t => t.League.Id == leagueId);
            foreach (var item in results)
            {
                items.Add(new SelectListItem { Text = Team.PrettyNameWithoutLeague(item), Value = item.Id.ToString() });
            }
            return items;
        }

        
        public IList<SelectListItem> GetUmpiresList()
        {
            var items = new List<SelectListItem>();
            var results = Umpire.GetAllUmpires();
            foreach (var item in results)
            {
                items.Add(new SelectListItem { Text = item.LastName + ", " + item.FirstName, Value = item.Id.ToString() });
            }
            return items;
        }

        
        public IList<SelectListItem> GetLocationsList()
        {
            var items = new List<SelectListItem>();
            var results = Location.GetAllLocations();
            foreach (var item in results)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        private GameListModel PopulateGamesList()
        {
            return PopulateGamesList(false);
        }

        public ActionResult GetAvailableDates(int teamId, int? divisionId, int? distance, int? locationId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Web.Models.Team.GetTeamById(teamId, user);
            var division = team.Division;
            if (divisionId.HasValue)
            {
                division = Division.GetDivisionById(divisionId.Value);
            }
            var location = team.Location;
            if (locationId.HasValue)
            {
                location = Location.GetLocationById(locationId.Value);
            }
            var miles = 50;
            if(distance.HasValue) {
                miles = distance.Value;
            }
            var availableDates = AvailableDates.GetAvailableDatesForTeamToPick(team, division, miles, location);
            var ret = new TeamAvailableDates
            {
                TeamId = team.Id,
                AvailableDates = AvailableDateResultDTO.MapAvailableDateResults(availableDates)
            };
            var info = PopulateGamesList();
            return PartialView("_AvailableDatesGrid", ret);
        }

        public ActionResult PlayAvailableDate(int availableDateId, int teamId, int locationId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Web.Models.Team.GetTeamById(teamId, user);
            var location = Location.GetLocationById(locationId);
            return null;// PartialView("_AvailableDatesGrid", location);
        }

        private GameListModel PopulateGamesList(bool OnlyGamesWithoutUmpires)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var model = new GameListModel();
            model.UserRole = user.Role;
            var teams = Team.GetTeamsForUser(user);
            var games = Game.GetGamesForUser(user);
            if (OnlyGamesWithoutUmpires)
            {
                games = games.Where<Game>(g => g.PlateUmpire == null || g.FieldUmpire == null).ToList();
            }
            model.Games = games;
            model.GamesInfo = new List<GameInfoModel>();
            foreach (var game in games.OrderByDescending(g => g.GameDate).ToList())
            {
                model.GamesInfo.Add(Game.MapGameToGameInfo(game));
            }

            model.TeamGames = new List<TeamGameModel>();
            foreach (var team in teams)
            {
                var teamGames = new TeamGameModel();
                teamGames.Team = team;
                teamGames.Games = model.Games.Where(g => g.HomeTeam == team || g.AwayTeam == team).ToList();
                teamGames.GamesInfo = new List<GameInfoModel>();
                foreach (var game in teamGames.Games.OrderByDescending(g => g.GameDate).ToList())
                {
                    teamGames.GamesInfo.Add(Game.MapGameToGameInfo(game));
                }

                var availableDates = AvailableDates.GetAvailableDatesForTeamToPick(team, team.Division, 50, team.Location);
                teamGames.AvailableDates = new TeamAvailableDates
                {
                    TeamId = team.Id,
                    AvailableDates = AvailableDateResultDTO.MapAvailableDateResults(availableDates)
                };
                model.TeamGames.Add(teamGames);
            }

            return model;
        }

        public ActionResult Index()
        {
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Teams = GetTeamsList();
            ViewBag.FilterOptions = GetFilterOptions();
            ViewBag.Locations = GetLocationsList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.Distances = GetDistancesList();
            return View(PopulateGamesList());
        }

        public ActionResult GamesWithoutUmpires()
        {
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Teams = GetTeamsList();
            return View("Index", PopulateGamesList(true));
        }

        

        public ActionResult GamesInfo(int? leagueId, string FilterOptions)
        {
            var model = PopulateGamesList();
            if (FilterOptions != null)
            {
                if (FilterOptions == FilterOptions_NoPlateUmpire)
                    model.GamesInfo = model.GamesInfo.Where(g => g.Game.PlateUmpire == null).ToList();
                if (FilterOptions == FilterOptions_NoFieldUmpire)
                    model.GamesInfo = model.GamesInfo.Where(g => g.Game.FieldUmpire == null).ToList();
                if (FilterOptions == FilterOptions_NoUmpires)
                    model.GamesInfo = model.GamesInfo.Where(g => g.Game.FieldUmpire == null || g.Game.PlateUmpire == null).ToList();
            }
            if (leagueId.HasValue)
            {
                model.GamesInfo = model.GamesInfo.Where(g => (g.Game.HomeTeam != null && g.Game.HomeTeam.League.Id == leagueId) || 
                    (g.Game.AwayTeam != null && g.Game.AwayTeam.League.Id == leagueId) ||
                    (g.Game.Bracket == null ? false : g.Game.Bracket.League.Id == leagueId)).ToList();
            }
            return PartialView("_GamesGrid", model.GamesInfo);
        }

        [HttpPost]
        public ActionResult GetAvailableDatesForTeam(int teamId)
        {
            var resp = new AvailableDatesResponse();
            resp.Success = false;

            try
            {
                var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                var team = Team.GetTeamById(teamId);
                var dates = AvailableDates.GetAvailableDatesForTeam(team);
                var datesDTO = new List<AvailableDateDTO>();
                foreach (var date in dates)
                {
                    datesDTO.Add(new AvailableDateDTO
                    {
                        Id = date.Id,
                        Date = date.Date.ToString(),
                        Location = date.Location.Name
                    });
                }
                resp.Dates = datesDTO;
                resp.Success = true;
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                resp.Error = "An error occurred while trying to retrieve the available dates for the selected team.";
            }
            return Json(resp);
        }

        [AuthorizeRoles(UserRole.Coach, UserRole.Manager)]
        public ActionResult RequestGame(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            var team = teams.Where(t => t.Id == id).SingleOrDefault();
            if (team == null)
                return RedirectToAction("Index", "Team");

            var awayTeams = team.League.Teams.Where(t => t != team).OrderByDescending(t => t.Division.MaxAge == team.Division.MaxAge).ThenBy(t => t.Division.MaxAge).ThenBy(t => t.Name);
            ViewBag.AwayTeams = CreateListFromTeams(awayTeams.ToList());
            var model = new GameRequestModel();
            model.HomeTeamId = team.Id;
            return View(model);
        }

        [AuthorizeRoles(UserRole.Coach, UserRole.Manager)]
        [HttpPost]
        public ActionResult RequestGame(GameRequestModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            var team = teams.Where(t => t.Id == model.HomeTeamId).SingleOrDefault();
            if (team == null)
                return RedirectToAction("Index", "Team");

            var awayTeams = team.League.Teams.Where(t => t != team).OrderByDescending(t => t.Division.MaxAge == team.Division.MaxAge).ThenBy(t => t.Division.MaxAge).ThenBy(t => t.Name);
            ViewBag.AwayTeams = CreateListFromTeams(awayTeams.ToList());

            if (!ModelState.IsValid)
                return View(model);

            var awayTeam = awayTeams.Where(t => t.Id == model.AwayTeamId).Single();
            var awayTeamDates = AvailableDates.GetAvailableDatesForTeam(awayTeam);
            var details = awayTeamDates.Where(t => t.Id == model.AvailableDateId).Single();

            Game.ScheduleAwayGame(team, awayTeam, user, details);
            TempData["GameRequested"] = true;
            return RedirectToAction("RequestGame", "Game");
        }

        [AuthorizeRoles(UserRole.Coach, UserRole.Manager)]
        public ActionResult RequestSpecificGame(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            var team = teams.Where(t => t.Id == id).SingleOrDefault();
            if (team == null)
                return RedirectToAction("Index", "Team");

            ViewBag.Locations = GetLocationsList();
            var awayTeams = team.League.Teams.Where(t => t != team).OrderByDescending(t => t.Division.MaxAge == team.Division.MaxAge).ThenBy(t => t.Division.MaxAge).ThenBy(t => t.Name);
            ViewBag.AwayTeams = CreateListFromTeams(awayTeams.ToList());
            var model = new GameSpecificRequestModel();
            model.HomeTeamId = team.Id;
            return View(model);
        }

        [AuthorizeRoles(UserRole.Coach, UserRole.Manager)]
        [HttpPost]
        public ActionResult RequestSpecificGame(GameSpecificRequestModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            var team = teams.Where(t => t.Id == model.HomeTeamId).SingleOrDefault();
            if (team == null)
                return RedirectToAction("Index", "Team");

            ViewBag.Locations = GetLocationsList();
            var awayTeams = team.League.Teams.Where(t => t != team).OrderByDescending(t => t.Division.MaxAge == team.Division.MaxAge).ThenBy(t => t.Division.MaxAge).ThenBy(t => t.Name);
            ViewBag.AwayTeams = CreateListFromTeams(awayTeams.ToList());

            if (!ModelState.IsValid)
                return View(model);

            var awayTeam = awayTeams.Where(t => t.Id == model.AwayTeamId).Single();
            
            Game.ScheduleAwayGame(team, awayTeam, user, Location.GetLocationById(model.LocationId), DateTime.Parse(model.GameDate + ' ' + model.GameTime));
            TempData["GameRequested"] = true;
            return RedirectToAction("RequestSpecificGame", "Game");
        }
        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult MassCreate()
        {
            // administrator will choose a league/tournament and division, which will automatically populate a grid of games to be scheduled
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.Locations = GetLocationsList();
            var model = new GameMassModel();
            return View(model);
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult GetTeamsForLeagueAndDivision(int leagueId, int divisionId)
        {
            var resp = new LeagueTeamsResponse();
            resp.Success = false;

            try
            {
                var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                var league = League.GetLeagueById(leagueId);
                var division = Division.GetDivisionById(divisionId);
                resp.LeagueType = league.Type;
                resp.Teams = new List<LeagueTeamDTO>();
                var teams = league.Teams.Where(t => t.Division == division && t.League == league).ToList();
                foreach (var team in teams)
                {
                    resp.Teams.Add(new LeagueTeamDTO
                    {
                         TeamId = team.Id,
                         TeamName = team.Name
                    });
                }

                resp.Success = true;
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                resp.Error = "An error occurred while trying to retrieve the teams for the league and division.";
            }
            return Json(resp);
        }

        [AuthorizeRoles(UserRole.Administrator)]
        [HttpPost]
        public ActionResult MassCreate(GameMassModel model)
        {
            // administrator will choose a league/tournament and division, which will automatically populate a grid of games to be scheduled
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.Locations = GetLocationsList();

            var league = League.GetLeagueById(model.LeagueId);
            var division = Division.GetDivisionById(model.DivisionId);
            var teams = league.Teams.Where(t => t.Division == division && t.League == league).ToList();
            model.Teams = teams;
            ViewBag.Teams = CreateListFromTeams(teams);

            if (!ModelState.IsValid)
                return View(model);

            using (var tx = session.BeginTransaction())
            {
                foreach (var item in model.Games)
                {
                    var game = new Game();
                    game.HomeTeam = Team.GetTeamById(item.HomeTeamId);
                    game.AwayTeam = Team.GetTeamById(item.AwayTeamId);
                    game.GameDate = DateTime.Parse(item.GameDate + ' ' + item.GameTime);
                    game.Location = Location.GetLocationById(item.LocationId);
                    game.Field = item.Field;
                    game.CreatedBy = user;
                    game.CreatedOn = DateTime.Now;
                    game.Status = GameStatus.Requested;
                    game.TeamToConfirmGame = game.AwayTeam;
                    game.TeamToRequestGame = game.HomeTeam;
                    game.Division = game.HomeTeam.Division.MaxAge > game.AwayTeam.Division.MaxAge ? game.HomeTeam.Division : game.AwayTeam.Division;

                    session.Save(game);
                }
                tx.Commit();

                TempData["GamesCreated"] = true;
                return RedirectToAction("Index");
            }            

            //return View(model);
        }


        public ActionResult GetMyTeams(int LeagueId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            return Json(GetMyTeamsList(user, LeagueId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllTeams(int LeagueId)
        {
            return Json(GetTeamsList(LeagueId), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult Create()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new GameNewModel();
            ViewBag.Leagues = GetLeagueList();
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.Innings = GetInningsList();
            model.GameDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToShortDateString();
            model.GameTime = "5:00pm";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult Create(GameNewModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Leagues = GetLeagueList();
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var game = new Game();
                game.HomeTeam = session.Load<Team>(model.HomeTeamId);
                game.AwayTeam = session.Load<Team>(model.AwayTeamId);
                game.GameDate = DateTime.Parse(model.GameDate + ' ' + model.GameTime);
                game.Location = session.Load<Location>(model.LocationId);
                game.Innings = model.Innings;
                game.Field = model.Field;
                game.CreatedBy = user;
                game.CreatedOn = DateTime.Now;
                game.Status = GameStatus.Requested;
                game.TeamToConfirmGame = game.AwayTeam;
                game.TeamToRequestGame = game.HomeTeam;
                game.Division = game.HomeTeam.Division.MaxAge > game.AwayTeam.Division.MaxAge ? game.HomeTeam.Division : game.AwayTeam.Division;

                session.Save(game);
                tx.Commit();
                 
                TempData["GameCreated"] = true;
                return RedirectToAction("Index");
            }
        }


        public ActionResult Edit(int id = 0, string returnUrl = null)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new GameUpdateModel();
            model.ReturnUrl = returnUrl;
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.IsBrackets = false;

            var item = Game.GetGameById(id, user);
            if (item == null)
                return HttpNotFound();


            if (item.Bracket == null || item.BracketBracket == null)
            {
                //    return RedirectToAction("Games", "Brackets", new { id = item.Bracket.Id });
                model.HomeTeamId = item.HomeTeam.Id;
                model.AwayTeamId = item.AwayTeam.Id;
                ViewBag.IsBrackets = true;
            }
            model.GameDate = item.GameDate.ToShortDateString();
            model.GameTime = item.GameDate.ToString("h:mmtt");
            model.LocationId = item.Location.Id;
            model.Innings = item.Innings;
            model.Field = item.Field;
            //model.HomeTeamScore = item.HomeTeamScore;
            //model.AwayTeamScore = item.AwayTeamScore;
            //model.PlateUmpireId = item.PlateUmpire.Id;
            //model.FieldUmpireId = item.FieldUmpire.Id;
            model.Status = item.Status;
            return View(model);
        }

        public ActionResult EditUmpires(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new GameUpdateUmpireModel();
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            var item = Game.GetGameById(id, user);
            if (item == null)
                return HttpNotFound();

            if(item.PlateUmpire != null)
                model.PlateUmpireId = item.PlateUmpire.Id;
            if (item.FieldUmpire != null)
                model.FieldUmpireId = item.FieldUmpire.Id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUmpires(GameUpdateUmpireModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            if (!ModelState.IsValid)
                return View(model);

            var item = Game.GetGameById(model.Id, user);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.PlateUmpire = session.Load<Umpire>(model.PlateUmpireId);
                item.FieldUmpire = session.Load<Umpire>(model.FieldUmpireId);
                session.Update(item);
                tx.Commit();
            }

            TempData["GameUpdated"] = true;
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GameUpdateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Leagues = GetLeagueList();
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            if (!ModelState.IsValid)
                return View(model);

            var item = Game.GetGameById(model.Id, user);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                if (item.Bracket == null || item.BracketBracket == null)
                {
                    item.HomeTeam = session.Load<Team>(model.HomeTeamId);
                    item.AwayTeam = session.Load<Team>(model.AwayTeamId);
                }
                item.GameDate = DateTime.Parse(model.GameDate + ' ' + model.GameTime);
                item.Location = session.Load<Location>(model.LocationId);
                item.Innings = model.Innings;
                item.Status = model.Status;
                item.Field = model.Field;
                session.Update(item);
                tx.Commit();
            }

            if (model.NotifyTeams != null)
            {
                EmailNotification.GameUpdateNotification(item);
            }

            TempData["GameUpdated"] = true;
            if (model.ReturnUrl != null)
                return Redirect(model.ReturnUrl);
            else
                return RedirectToAction("Index");
        }


        public ActionResult EditScore(int id = 0, string returnUrl = null)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new GameUpdateScoreModel();
            model.ReturnUrl = returnUrl;
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            var item = Game.GetGameById(id, user);
            if (item == null)
                return HttpNotFound();

            model.HomeTeamScore = item.HomeTeamScore;
            model.AwayTeamScore = item.AwayTeamScore;
            model.Status = item.Status;
            model.Game = item;

            var stats = PlayerGameStat.GetPlayerGameStatsForGame(item);
            model.HomePlayerStats = new List<GameUpdatePlayerStatModel>();
            model.AwayPlayerStats = new List<GameUpdatePlayerStatModel>();
            if (item.HomeTeam != null)
            {
                foreach (var teamPlayer in item.HomeTeam.Players)
                {
                    model.HomePlayerStats.Add(new GameUpdatePlayerStatModel
                    {
                        Player = teamPlayer,
                        PlayerId = teamPlayer.Id,
                        InningsPitched = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.InningsPitched).SingleOrDefault(),
                        InningsOuts = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.InningsOuts).SingleOrDefault(),
                        PitchesThrown = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.PitchesThrown).SingleOrDefault()
                    });
                }
            }

            if (item.AwayTeam != null)
            {
                foreach (var teamPlayer in item.AwayTeam.Players)
                {
                    model.AwayPlayerStats.Add(new GameUpdatePlayerStatModel
                    {
                        Player = teamPlayer,
                        PlayerId = teamPlayer.Id,
                        InningsPitched = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.InningsPitched).SingleOrDefault(),
                        InningsOuts = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.InningsOuts).SingleOrDefault(),
                        PitchesThrown = stats.Where(s => s.TeamPlayer == teamPlayer).Select(s => s.PitchesThrown).SingleOrDefault()
                    });
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditScore(GameUpdateScoreModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.MyTeams = GetMyTeamsList(user);
            ViewBag.AllTeams = GetTeamsList();
            ViewBag.Innings = GetInningsList();
            ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsList();

            var item = Game.GetGameById(model.Id, user);
            if (item == null)
                return RedirectToAction("Index");

            model.Game = item;
            
            // todo: repopulate .Player in the stats model in case the modelstate is invalid

            if (!ModelState.IsValid)
                return View(model);

            using (var tx = session.BeginTransaction())
            {
                item.HomeTeamScore = model.HomeTeamScore;
                item.AwayTeamScore = model.AwayTeamScore;
                item.Status = model.Status;

                if (item.HomeTeam != null && item.AwayTeam != null)
                {
                    var stats = PlayerGameStat.GetPlayerGameStatsForGame(item);
                    var allStats = new List<GameUpdatePlayerStatModel>();

                    if (model.HomePlayerStats != null)
                    {
                        allStats.AddRange(model.HomePlayerStats.ToList());
                    }
                    if (model.AwayPlayerStats != null)
                    {
                        allStats.AddRange(model.AwayPlayerStats.ToList());
                    }
                    foreach (var stat in allStats)
                    {
                        var teamPlayer = item.HomeTeam.Players.Where(p => p.Id == stat.PlayerId).SingleOrDefault();
                        if (teamPlayer == null)
                            teamPlayer = item.AwayTeam.Players.Where(p => p.Id == stat.PlayerId).SingleOrDefault();

                        if (teamPlayer != null)
                        {
                            var existingStat = stats.Where(s => s.TeamPlayer == teamPlayer).SingleOrDefault();
                            if (existingStat != null)
                            {
                                existingStat.InningsPitched = stat.InningsPitched;
                                existingStat.InningsOuts = stat.InningsOuts;
                                existingStat.PitchesThrown = stat.PitchesThrown;
                                session.Update(existingStat);
                            }
                            else
                            {
                                var newStat = new PlayerGameStat();
                                newStat.Game = item;
                                newStat.TeamPlayer = teamPlayer;
                                newStat.InningsPitched = stat.InningsPitched;
                                newStat.InningsOuts = stat.InningsOuts;
                                newStat.PitchesThrown = stat.PitchesThrown;
                                session.Save(newStat);
                            }
                        }
                    }
                }

                session.Update(item);
                tx.Commit();
            }

            TempData["GameUpdated"] = true;
            if (model.ReturnUrl != null)
                return Redirect(model.ReturnUrl);
            else
                return RedirectToAction("Index");
        }

        public ActionResult Delete(int id, string returnUrl = null)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            using (var tx = session.BeginTransaction())
            {
                var item = Game.GetGameById(id, user);
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["GameDeleted"] = true;
            if (returnUrl != null)
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index");
        }

        public ActionResult GameApprove(int id)
        {
            SetGameStatus(id, GameStatus.Confirmed);
            EmailNotification.GamesConfirmed(Game.GetGameById(id), Game.GetGameById(id).TeamToRequestGame);
            return RedirectToAction("Index");
        }

        public ActionResult GameDecline(int id)
        {
            SetGameStatus(id, GameStatus.Declined);
            return RedirectToAction("Index");
        }

        public void SetGameStatus(int id, GameStatus status)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            using (var tx = session.BeginTransaction())
            {
                var item = Game.GetGameById(id, user);
                if (item != null)
                {
                    item.Status = status;
                    session.Update(item);
                }
                tx.Commit();
            }
        }

        public static List<SelectListItem> GetLocationsListForAvailableDate(AvailableDates availableDate, Team userTeam)
        {
            if (userTeam.Location == null)
                return new List<SelectListItem>();

            var availableLocations = new List<AvailableLocationResult>();

            // if an Away game, get locations within the distance to the home location
            var locations = Location.GetAllLocations();
            var homeLocation = Microsoft.SqlServer.Types.SqlGeography.Point(availableDate.Location.Longitude, availableDate.Location.Latitude, 4326);
            // a lot of teams do not have a home location, so this is not very useful
            //Microsoft.SqlServer.Types.SqlGeography.Point teamHomeLocation = null;
            //if(userTeam.Location != null) {
            //  teamHomeLocation = Microsoft.SqlServer.Types.SqlGeography.Point(userTeam.Location.Longitude, userTeam.Location.Latitude, 4326);
            //}
            if (availableDate.IsAway)
            {
                availableLocations = (from l in locations
                                      where l != availableDate.Location && (double)Microsoft.SqlServer.Types.SqlGeography.Point(l.Longitude, l.Latitude, 4326)
                                                              .STDistance(homeLocation) <= (availableDate.DistanceFromLocation.Value * 1609.34)
                                      select new AvailableLocationResult
                                      {
                                          Location = l,
                                          Distance = (double)Microsoft.SqlServer.Types.SqlGeography.Point(l.Longitude, l.Latitude, 4326)
                                                              .STDistance(homeLocation)
                                      }).ToList();
            }
            // available to play at home
            if (availableDate.IsHome)
            {
                availableLocations.Add(new AvailableLocationResult { Location = availableDate.Location, Distance = 0 });
            }

            var locationList = new List<SelectListItem>();
            availableLocations = availableLocations.OrderBy(l => l.Distance).ToList();
            foreach (var location in availableLocations)
            {
                bool selected = false;
                if (availableDate.IsAway)
                {
                    // choose the user's home location, if available
                    if (location.Location.Id == userTeam.Location.Id)
                        selected = true;
                }
                else
                {
                    // choose the available date location, if available
                    if (location.Location.Id == availableDate.Location.Id)
                        selected = true;
                }
                locationList.Add(new SelectListItem { Value = location.Location.Id.ToString(), Text = string.Format("{0} ({1} miles)", location.Location.Name, (location.Distance / 1609.34).ToString("F1")), Selected = selected });
            }
            return locationList.Distinct().ToList();
        }

        public ActionResult Challenge(int id, int myTeamId)
        {
            var availableDate = AvailableDates.GetAvailableDateById(id);
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var viewModel = new GameChallengeNewGameModel();
            viewModel.AvailableDate = availableDate;
            var userTeam = Team.GetTeamById(myTeamId);
            ViewBag.MyTeam = userTeam;
            ViewBag.Locations = GetLocationsListForAvailableDate(availableDate, userTeam);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Challenge(int id, int myTeamId, GameChallengeNewGameModel model)
        {
            var availableDate = AvailableDates.GetAvailableDateById(id);
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var userTeam = Team.GetTeamById(myTeamId);
            model.AvailableDate = availableDate;
            ViewBag.MyTeam = userTeam;
            //ViewBag.Leagues = GetLeagueList();
            //ViewBag.MyTeams = GetMyTeamsList(user);
            //ViewBag.AllTeams = GetTeamsList();
            //ViewBag.Innings = GetInningsList();
            //ViewBag.Umpires = GetUmpiresList();
            ViewBag.Locations = GetLocationsListForAvailableDate(availableDate, userTeam);

            if (!ModelState.IsValid)
                return View(model);

            using (var tx = session.BeginTransaction())
            {
                var location = Location.GetLocationById(model.LocationId);
                var game = new Game();
                if (availableDate.IsHome && !availableDate.IsAway)
                {
                    game.HomeTeam = availableDate.Team;
                    game.Location = availableDate.Location;
                    game.AwayTeam = userTeam;
                }
                else if (availableDate.IsAway && !availableDate.IsHome)
                {
                    game.AwayTeam = availableDate.Team;
                    game.Location = location;
                    game.HomeTeam = userTeam;
                }
                else
                {
                    // home or away - determined by chosen location
                    if (location == availableDate.Location)
                    {
                        game.HomeTeam = availableDate.Team;
                        game.AwayTeam = userTeam;
                    }
                    else
                    {
                        game.HomeTeam = userTeam;
                        game.AwayTeam = availableDate.Team;
                    }
                    game.Location = location;

                }
                game.CreatedBy = user;
                game.CreatedOn = DateTime.Now;
                game.GameDate = availableDate.Date;
                game.Status = GameStatus.Requested;
                game.TeamToRequestGame = userTeam;
                game.TeamToConfirmGame = availableDate.Team;
                game.Division = game.HomeTeam.Division.MaxAge > game.AwayTeam.Division.MaxAge ? game.HomeTeam.Division : game.AwayTeam.Division;

                session.Save(game);
                tx.Commit();

                EmailNotification.GamesRequested(game, user, userTeam, availableDate.Team);

                TempData["GameCreated"] = true;
                return RedirectToAction("Index");
            }

        }

    }
}