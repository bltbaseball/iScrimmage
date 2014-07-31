using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator)]
    public class BracketsController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

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

        public IList<SelectListItem> CreateListFromTeams(IList<Team> list)
        {
            var items = new List<SelectListItem>();
            foreach (var item in list)
            {
                items.Add(new SelectListItem { Text = Team.PrettyNameWithoutLeague(item), Value = item.Id.ToString() });
            }
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

        public IList<SelectListItem> GetTeamList(IList<Team> teams)
        {
            var items = new List<SelectListItem>();
            foreach (var item in teams)
            {
                items.Add(new SelectListItem { Text = Team.PrettyNameWithoutLeague(item), Value = item.Id.ToString() });
            }
            return items;
        }


        //
        // GET: /Division/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var model = new BracketUpdateModel();
            var item = session.Get<Bracket>(id);
            if (item == null)
                return HttpNotFound();

            model.Name = item.Name;
            return View(model);
        }

        //
        // POST: /Division/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BracketUpdateModel model)
        {
            ViewBag.Leagues = GetLeagueList();
            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<Bracket>(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.Name = model.Name;
                session.Update(item);
                tx.Commit();
            }

            TempData["BracketUpdated"] = true;
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            if (bracket == null)
                return RedirectToAction("Index");
            try
            {
                Bracket.DeleteBracket(bracket);
                TempData["BracketDeleted"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                if (e is ApplicationException)
                {
                    TempData["Error"] = e.Message;
                }
                else
                {
                    TempData["Error"] = "An error occurred while trying to delete the bracket.";
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            if (bracket == null)
                return RedirectToAction("Index");

            return View(Bracket.PopulateBracketInfo(bracket));
        }

        public ActionResult Index()
        {
            var model = new BracketsListModel();
            model.Brackets = Bracket.GetAllBrackets().ToList();
            return View("List", model);
        }

        public ActionResult Create()
        {
            var model = new BracketNewModel();
            ViewBag.Leagues = GetLeagueList();
            model.Teams = new List<BracketTeamModel>();
            return View(model);
        }

        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult SubmitPool(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            if (bracket == null)
                return RedirectToAction("Index");
            var model = new BracketSubmitPoolModel();
            model.Bracket = bracket;
            model.PoolRankings = Bracket.Rankings(bracket, true);
            return View(model);
        }

        [AuthorizeRoles(UserRole.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitPool(BracketSubmitPoolModel model, int id)
        {
            var bracket = Bracket.GetBracketById(id);
            var info = Bracket.PopulateBracketInfo(bracket);
            var bracketTeams = BracketTeam.GetBracketTeams(bracket);
            using (var ts = session.BeginTransaction())
            {
                foreach (var team in model.Teams)
                {
                    var bracketTeam = bracketTeams.Where(g => g.Standing == team.Standing).Single();
                    bracketTeam.Team = Team.GetTeamById(team.TeamId);
                    session.SaveOrUpdate(bracketTeam);
                }
                ts.Commit();
            }
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult UnsubmitPool(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            var info = Bracket.PopulateBracketInfo(bracket);
            var bracketTeams = BracketTeam.GetBracketTeams(bracket);
            using (var ts = session.BeginTransaction())
            {
                foreach (var team in bracketTeams)
                {
                    team.Team = null;
                    session.SaveOrUpdate(team);
                }
                ts.Commit();
            }
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult NotifyTeamsOfPoolGames(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            var info = Bracket.PopulateBracketInfo(bracket);
            var games = Game.GetPoolGamesForBracket(bracket);
            foreach (var game in games)
            {
                EmailNotification.GamePlayNotification(game);
            }
            TempData["Notice"] = "Teams have been notified of pool games via email";
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult NotifyTeamsOfBracketGames(int id)
        {
            var bracket = Bracket.GetBracketById(id);
            var info = Bracket.PopulateBracketInfo(bracket);
            var games = Game.GetBracketGamesForBracket(bracket);
            foreach (var game in games)
            {
                EmailNotification.GamePlayNotification(game);
            }
            TempData["Notice"] = "Teams have been notified of bracket games via email";
            return RedirectToAction("Index");
        }

        public ActionResult CreatePool()
        {
            var model = new BracketPoolGameModel();
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.Locations = GetLocationsList();
            return View(model);
        }

        [AuthorizeRoles(UserRole.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePool(BracketPoolGameModel model)
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

            foreach (var item in model.Games)
            {
                foreach (var item2 in model.Games)
                {
                    if (item != item2)
                    {
                        if (item.HomeTeamId == item2.HomeTeamId || item.HomeTeamId == item2.AwayTeamId)
                        {
                            if (item.GameDate == item2.GameDate && item.GameTime == item2.GameTime ||
                                item.GameDate == item2.GameDate && item.GameTime == item2.GameTime)
                            {
                                TempData["Error"] = "Team " + Team.PrettyNameWithoutLeague(Team.GetTeamById(item.HomeTeamId)) + " has games at the same time";
                                return View(model);
                            }
                        }
                        if (item.AwayTeamId == item2.HomeTeamId || item.AwayTeamId == item2.AwayTeamId)
                        {
                            if (item.GameDate == item2.GameDate && item.GameTime == item2.GameTime ||
                                item.GameDate == item2.GameDate && item.GameTime == item2.GameTime)
                            {
                                TempData["Error"] = "Team " + Team.PrettyNameWithoutLeague(Team.GetTeamById(item.AwayTeamId)) + " has games at the same time";
                                return View(model);
                            }
                        }
                    }
                }
            }
            using (var tx = session.BeginTransaction())
            {
                var bracket = new Bracket();
                bracket.Name = model.Name;
                bracket.Division = division;
                bracket.League = league;
                bracket.CreatedOn = DateTime.Now;
                bracket.Standings = new List<BracketTeam>();
                session.Save(bracket);

                // create bracket with null teams -- to be populated later
                for (var i = 0; i < model.Teams.Count; i++)
                {
                    var bracketTeam = new BracketTeam();
                    bracketTeam.Team = null;
                    bracketTeam.Standing = (i + 1);
                    bracketTeam.Bracket = bracket;
                    session.Save(bracketTeam);
                    bracket.Standings.Add(bracketTeam);
                }
                session.Update(bracket);

                foreach (var item in model.Games)
                {
                    var game = new Game();
                    game.Bracket = bracket;
                    game.HomeTeam = Team.GetTeamById(item.HomeTeamId);
                    game.AwayTeam = Team.GetTeamById(item.AwayTeamId);
                    game.GameDate = DateTime.Parse(item.GameDate + ' ' + item.GameTime);
                    game.Location = Location.GetLocationById(item.LocationId);
                    game.Field = item.Field;
                    game.CreatedBy = user;
                    game.CreatedOn = DateTime.Now;
                    game.Status = GameStatus.Confirmed;
                    game.Division = bracket.Division;

                    session.Save(game);
                }
                tx.Commit();

                TempData["BracketCreated"] = true;
                return RedirectToAction("Games", new { id = bracket.Id });
            }
        }

        public ActionResult Games(int id)
        {
            ViewBag.Locations = GetLocationsList();
            var bracket = Bracket.GetBracketById(id);
            if (bracket == null)
                return RedirectToAction("Index");

            var info = Bracket.PopulateBracketInfo(bracket);
            
            var model = new BracketGamesModel();
            model.Id = id;
            model.Info = info;
            model.Games = info.Games.Where(g => g.Game != null).OrderBy(g => g.GameNumber).ToList(); //new List<BracketGameModel>();
            var poolGames = Game.GetPoolGamesForBracket(bracket);
            model.PoolGames = new List<GameUpdateModel>();

            
            foreach (var item in poolGames)
            {
                var gameInfo = new GameUpdateModel
                {
                    AwayTeamId = item.AwayTeam.Id,
                    HomeTeamId = item.HomeTeam.Id,
                    Field = item.Field,
                    GameDate = item.GameDate.ToShortDateString(),
                    GameTime = item.GameDate.ToString("h:mmtt"),
                    Id = item.Id,
                    LocationId = item.Location.Id,
                    Status = item.Status,
                    Game = item
                };
                model.PoolGames.Add(gameInfo);
            }

            /*foreach (var item in info.Games)
            {
                if (item.IsTeam1Bye || item.IsTeam2Bye)
                    continue;
                var gameInfo = new BracketGameModel
                {
                    Id = item.Id,
                    Bracket = item.Bracket,
                    Position = item.Position,
                    Team1 = item.Team1,
                    Team2 = item.Team2,
                    Team1Seed = item.Team1Seed,
                    Team2Seed = item.Team2Seed,
                    GameNumber = item.GameNumber
                };
                var game = Game.GetGameByBracket(bracket, item.Bracket, item.Position);
                if (game != null)
                {
                    gameInfo.LocationId = game.Location.Id;
                    gameInfo.Field = game.Field;
                    gameInfo.GameDate = game.GameDate.ToShortDateString();
                    gameInfo.GameTime = game.GameDate.ToString("h:mmtt");
                    gameInfo.Game = game;
                }
                model.Games.Add(gameInfo);
            }*/
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Games(BracketGamesModel model)
        {
            ViewBag.Locations = GetLocationsList();
            var bracket = Bracket.GetBracketById(model.Id);
            if (bracket == null)
                return RedirectToAction("Index");

            var info = Bracket.PopulateBracketInfo(bracket);

            //if (!ModelState.IsValid)
            //    return View(model);

            // create some games!
            using (var ts = session.BeginTransaction())
            {
                if (model.Games != null)
                {
                    foreach (var item in model.Games)
                    {
                        // update if an existing game, otherwise create new
                        var game = Game.GetGameByBracket(bracket, item.Bracket, item.Position);
                        var bracketInfo = info.Games.Where(g => g.Bracket == item.Bracket && g.Position == item.Position).Single();
                        if (game == null)
                        {
                            // don't create games for bye weeks
                            if (bracketInfo.IsTeam1Bye || bracketInfo.IsTeam2Bye)
                                continue;
                            game = new Game();
                            game.Bracket = bracket;
                            game.HomeTeam = bracketInfo.Team1;
                            game.AwayTeam = bracketInfo.Team2;
                            game.BracketPosition = item.Position;
                            game.BracketBracket = item.Bracket;
                            game.CreatedOn = DateTime.Now;
                            game.Status = GameStatus.Confirmed;
                            game.Division = bracket.Division;
                        }
                        game.GameDate = DateTime.Parse(item.GameDate + ' ' + item.GameTime);
                        game.Location = Location.GetLocationById(item.LocationId);
                        game.Field = item.Field;
                        session.SaveOrUpdate(game);
                    }
                }

                if (model.PoolGames != null)
                {
                    foreach (var item in model.PoolGames)
                    {
                        // update if an existing game, otherwise create new
                        var game = Game.GetPoolGamesForBracket(bracket).Where(g => g.Id == item.Id).Single();
                        if (game == null)
                        {
                            continue;
                        }
                        game.GameDate = DateTime.Parse(item.GameDate + ' ' + item.GameTime);
                        game.Location = Location.GetLocationById(item.LocationId);
                        game.Field = item.Field;
                        session.SaveOrUpdate(game);
                    }
                }
                
                ts.Commit();

                TempData["GamesCreated"] = true;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BracketNewModel model)
        {
            ViewBag.Leagues = GetLeagueList();
            var league = League.GetLeagueById(model.LeagueId);

            if (league == null)
                ModelState.AddModelError("LeagueId", "The league/tournament you have selected does not exist.");

            if (model.Teams == null)
            {
                ModelState.AddModelError("", "You must set team standings for the bracket.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var tx = session.BeginTransaction())
            {
                var bracket = new Bracket();
                bracket.Name = model.Name;
                bracket.League = league;
                bracket.CreatedOn = DateTime.Now;
                bracket.Standings = new List<BracketTeam>();
                session.Save(bracket);

                foreach (var team in model.Teams)
                {
                    var bracketTeam = new BracketTeam();
                    bracketTeam.Team = Team.GetTeamById(team.TeamId);
                    bracketTeam.Standing = team.Standing;
                    bracketTeam.Bracket = bracket;
                    session.Save(bracketTeam);
                    bracket.Standings.Add(bracketTeam);
                }
                session.Update(bracket);

                // get a list of games that need to be created for the bracket and have the user fill in the information, similar to the mass game creation screen
                //var brackets = session.GetNamedQuery("BracketCreate")
                //.SetParameter("BracketId", id)
                //.List<BracketResult>().ToList();


                tx.Commit();
                TempData["BracketCreated"] = true;
                return RedirectToAction("Games", new { id = bracket.Id });
            }
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult GetTeamsForLeague(int leagueId)
        {
            var resp = new LeagueTeamsResponse();
            resp.Success = false;

            try
            {
                var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                var league = League.GetLeagueById(leagueId);
                resp.LeagueType = league.Type;
                resp.Teams = new List<LeagueTeamDTO>();
                foreach (var team in league.Teams)
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
                resp.Error = "An error occurred while trying to retrieve the teams for the league.";
            }
            return Json(resp);
        }
    }
}
