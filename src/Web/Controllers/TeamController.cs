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
    [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
    public class TeamController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        public IList<SelectListItem> GetLocationsList(int selectId = 0)
        {
            var items = new List<SelectListItem>();
            var results = Location.GetAllLocations();
            foreach (var item in results)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = (item.Id == selectId ? true : false) });
            }
            return items;
        }

        public IList<SelectListItem> GetAvailableDateTypes(string selected = "")
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Home Game", Value = "Home", Selected = ("Home" == selected ? true : false) });
            items.Add(new SelectListItem { Text = "Away Game", Value = "Away", Selected = ("Away" == selected ? true : false) });
            items.Add(new SelectListItem { Text = "Home or Away Game", Value = "HomeOrAway", Selected = ("HomeOrAway" == selected ? true : false) });
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

        public IList<SelectListItem> GetActiveLeagueList()
        {
            var items = new List<SelectListItem>();
            var leagues = session.QueryOver<League>().Where(l => l.RegistrationEndDate >= DateTime.Now && l.RegistrationStartDate <= DateTime.Now && l.IsActive == true).OrderBy(l => l.CreatedOn).Desc.List();
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
                items.Add(new SelectListItem { Text = Team.PrettyName(item), Value = item.Id.ToString() });
            }
            return items;
        }

        public List<int> GetTournamentIds()
        {
            return session.QueryOver<League>().Where(l => l.Type == LeagueType.Tournament).OrderBy(l => l.CreatedOn).Desc.List().Select(l => l.Id).ToList();
        }

        public IList<SelectListItem> GetDivisionList()
        {
            var items = new List<SelectListItem>();
            var divisions = session.QueryOver<Division>().OrderBy(l => l.MaxAge).Desc.List();
            foreach (var item in divisions)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        private IList<SelectListItem> GetTeamClassList()
        {
            var items = new List<SelectListItem>();
            var teamclasses = session.QueryOver<TeamClass>().OrderBy(l => l.Name).Desc.List();
            foreach (var item in teamclasses)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        public ActionResult Index()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var items = Team.GetTeamsForUser(user);
            var model = new TeamListModel();
            model.Teams = new List<TeamListingModel>();
            foreach (var item in items)
            {
                var team = new TeamListingModel();
                team.Team = item;
                team.IsPaid = FeePayment.GetFeesPaidStatus(item);
                model.Teams.Add(team);
            }
            return View(model);
        }

        public ActionResult Copy()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var model = new TeamCopyModel();
            ViewBag.Leagues = GetLeagueList();
            var teams = Team.GetTeamsForUser(user);
            ViewBag.Teams = GetTeamList(teams.OrderBy(t => t.Name).ToList());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(TeamCopyModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            ViewBag.Leagues = GetLeagueList();
            var teams = Team.GetTeamsForUser(user);
            ViewBag.Teams = GetTeamList(teams.OrderBy(t => t.Name).ToList());

            var team = Team.GetTeamById(model.TeamId.Value, user);
            if (team == null)
                ModelState.AddModelError("", "The selected team was not found.");

            if (!ModelState.IsValid)
                return View(model);

            var league = League.GetLeagueById(model.LeagueId.Value);

            if (team.League == league)
                ModelState.AddModelError("LeagueId", "You must choose a different League/Tournament than the one you are copying from.");

            if (!ModelState.IsValid)
                return View(model);

            Team.CopyTeam(team, league, user);
            TempData["TeamCopied"] = true;
            return RedirectToAction("Index");
        }
        
        public ActionResult Create()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            ViewBag.Teams = GetTeamList(teams.OrderBy(t => t.Name).ToList());

            var model = new TeamCreateCopyModel();
            if (user.Role == UserRole.Administrator)
                ViewBag.Leagues = GetLeagueList();
            else
                ViewBag.Leagues = GetActiveLeagueList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();
            model.Choice = "Create";
            model.Copy = new TeamCopyModel();
            model.Create = new TeamNewModel();
            model.Create.IsLookingForPlayers = true;
            model.Create.AvailableDates = new List<AvailableDateNewModel>() { }; //new AvailableDateNewModel{ Date="7/25/2013", LocationId= 1, Time="5:00 PM"}
            return View(model);
        }

        [HttpPost]
        public JsonResult GetLocations()
        {
            var items = Location.GetAllLocations();
            return Json(items);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamCreateCopyModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var teams = Team.GetTeamsForUser(user);
            ViewBag.Teams = GetTeamList(teams.OrderBy(t => t.Name).ToList());
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();

            if (!ModelState.IsValid)
                return View(model);

            switch (model.Choice)
            {
                case "Copy":
                    //teamid,leagueid
                    Team copyTeam = null;
                    if (!model.Copy.TeamId.HasValue)
                    {
                        ModelState.AddModelError("Copy.TeamId", "The Team field is required.");
                    }
                    else
                    {
                        copyTeam = Team.GetTeamById(model.Copy.TeamId.Value, user);
                    }

                    if (copyTeam == null)
                        ModelState.AddModelError("", "The selected team was not found.");

                    if (!model.Copy.LeagueId.HasValue)
                        ModelState.AddModelError("Copy.LeagueId", "The League or Tournament field is required.");

                    if (!ModelState.IsValid)
                        return View(model);

                    var league = League.GetLeagueById(model.Copy.LeagueId.Value);

                    if (copyTeam.League == league)
                        ModelState.AddModelError("Copy.LeagueId", "You must choose a different League/Tournament than the one you are copying from.");

                    if (!ModelState.IsValid)
                        return View(model);

                    Team.CopyTeam(copyTeam, league, user);
                    TempData["TeamCopied"] = true;
                    return RedirectToAction("Index");

                case "Create":
                default:
                    // name,divisionid,classid,leagueid,
                    // update db from model

                    if(string.IsNullOrEmpty(model.Create.Name))
                        ModelState.AddModelError("Create.Name", "The Name field is required.");

                    if(!model.Create.DivisionId.HasValue)
                        ModelState.AddModelError("Create.DivisionId", "The Division field is required.");

                    if (!model.Create.ClassId.HasValue)
                        ModelState.AddModelError("Create.ClassId", "The Class field is required.");

                    if (!model.Create.LeagueId.HasValue)
                        ModelState.AddModelError("Create.LeagueId", "The League or Tournament field is required.");

                    if (!ModelState.IsValid)
                        return View(model);

                    using (var tx = session.BeginTransaction())
                    {
                        var team = new Team();
                        team.Name = model.Create.Name;
                        team.League = session.Load<League>(model.Create.LeagueId.Value);
                        team.Division = session.Load<Division>(model.Create.DivisionId.Value);
                        team.Class = session.Load<TeamClass>(model.Create.ClassId.Value);
                        team.Location = Location.GetLocationById(model.Create.LocationId.Value);
                        if(!string.IsNullOrEmpty(model.Create.Url)) 
                        {
                            if (!model.Create.Url.StartsWith("http://"))
                                model.Create.Url = "http://" + model.Create.Url;
                            team.Url = model.Create.Url;
                        }
                        team.CreatedOn = DateTime.Now;
                        team.IsLookingForPlayers = model.Create.IsLookingForPlayers;
                        team.HtmlDescription = model.Create.HtmlDescription;
                        team.DatesAvailable = new List<AvailableDates>();
                        session.Save(team);

                        if (model.Create.AvailableDates != null)
                        {
                            foreach (var item in model.Create.AvailableDates)
                            {
                                var date = new AvailableDates();
                                date.Team = team;
                                date.Date = DateTime.Parse(string.Format("{0} {1}", item.Date, item.Time));
                                // don't add dates in the past
                                if (date.Date < DateTime.Now)
                                    continue;
                                date.GameScheduled = false;
                                date.Location = Location.GetLocationById(item.LocationId);
                                date.IsHome = (item.Type == "Home" || item.Type == "HomeOrAway");
                                date.IsAway = (item.Type == "Away" || item.Type == "HomeOrAway");
                                if (date.IsAway)
                                {
                                    date.DistanceFromLocation = item.Distance;
                                }
                                else
                                {
                                    date.DistanceFromLocation = null;
                                }
                                team.DatesAvailable.Add(date);
                                session.Save(date);
                            }
                        }

                        switch (user.Role)
                        {
                            case UserRole.Manager:
                                var manager = Manager.GetManagerForUser(user);
                                team.Managers = new List<Manager>();
                                team.Managers.Add(manager);
                                session.Update(team);
                                EmailNotification.NewTeam(team, manager);
                                break;
                            case UserRole.Coach:
                                var coach = Coach.GetCoachForUser(user);
                                team.Coaches = new List<Coach>();
                                team.Coaches.Add(coach);
                                session.Update(team);
                                EmailNotification.NewTeam(team, coach);
                                break;
                            default:
                                break;
                        }

                        tx.Commit();

                        TempData["TeamCreated"] = true;
                        if(team.League.Type == LeagueType.Scrimmage || team.League.Type == LeagueType.League)
                            return RedirectToAction("EditAvailableDates", "Team", new { id = team.Id });
                        return RedirectToAction("Create", "Player", new { id = team.Id });
                    }
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new TeamUpdateModel();

            var item = Team.GetTeamById(id, user);
            model.Team = item;
            if (item == null)
                return HttpNotFound();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();
            
            model.Name = item.Name;
            model.Url = item.Url;
            model.ClassId = item.Class.Id;
            model.DivisionId = item.Division.Id;
            model.LeagueId = item.League.Id;
            if(item.Location != null)
                model.LocationId = item.Location.Id;
            model.IsLookingForPlayers = item.IsLookingForPlayers;
            model.HtmlDescription = item.HtmlDescription;
            model.AvailableDates = new List<AvailableDateUpdateModel>();

            return View(model);
        }

        public ActionResult EditAvailableDates(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new TeamUpdateDatesModel();

            var item = Team.GetTeamById(id, user);
            model.Team = item;
            if (item == null)
                return HttpNotFound();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Leagues = GetLeagueList();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();

            model.Name = item.Name;
            model.Url = item.Url;
            model.ClassId = item.Class.Id;
            model.DivisionId = item.Division.Id;
            model.LeagueId = item.League.Id;
            if (item.Location != null)
                model.LocationId = item.Location.Id;
            model.IsLookingForPlayers = item.IsLookingForPlayers;
            model.HtmlDescription = item.HtmlDescription;
            model.AvailableDates = new List<AvailableDateUpdateModel>();
            if (item.DatesAvailable != null)
            {
                var datesUnconfirmed = item.DatesAvailable.Where(d => d.GetGameScheduled() != null).ToList();

                foreach (var date in item.DatesAvailable)
                {
                    // don't add dates in the past
                    if (date.Date < DateTime.Now)
                        continue;
                    //dont add confirmed dates
                    if (date.GetGameScheduled() != null)
                        continue;
                    var dateModel = new AvailableDateUpdateModel();
                    dateModel.Id = date.Id;
                    dateModel.LocationId = date.Location.Id;
                    dateModel.Date = date.Date.ToShortDateString();
                    dateModel.Time = date.Date.ToShortTimeString();
                    // next time use an enum, dummy
                    if (date.IsHome && date.IsAway)
                    {
                        dateModel.Type = "HomeOrAway";
                    }
                    else if (date.IsHome)
                    {
                        dateModel.Type = "Home";
                    }
                    else
                    {
                        dateModel.Type = "Away";
                    }
                    dateModel.Distance = date.DistanceFromLocation;
                    model.AvailableDates.Add(dateModel);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamUpdateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var team = Team.GetTeamById(model.Id, user);
            model.Team = team;
            if (team == null)
                return RedirectToAction("Index");

            ViewBag.Leagues = GetLeagueList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();
            if (!ModelState.IsValid)
                return View(model);


            using (var tx = session.BeginTransaction())
            {
                if (user.Role == UserRole.Administrator)
                {
                    team.League = session.Load<League>(model.LeagueId);
                    team.Division = session.Load<Division>(model.DivisionId);
                    team.Class = session.Load<TeamClass>(model.ClassId);
                }
                team.Name = model.Name;
                team.Url = model.Url;
                team.Location = session.Get<Location>(model.LocationId);
                team.IsLookingForPlayers = model.IsLookingForPlayers;
                team.HtmlDescription = model.HtmlDescription;
                session.Update(team);

                tx.Commit();
            }

            TempData["TeamUpdated"] = true;
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAvailableDates(TeamUpdateDatesModel model, String Submit = "")
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var team = Team.GetTeamById(model.Id, user);
            model.Team = team;
            if (team == null)
                return RedirectToAction("Index");

            ViewBag.Leagues = GetLeagueList();
            ViewBag.Locations = GetLocationsList();
            ViewBag.AvailableDateTypes = GetAvailableDateTypes();
            ViewBag.Divisions = GetDivisionList();
            ViewBag.TeamClasses = GetTeamClassList();
            ViewBag.Tournaments = GetTournamentIds();
            if (!ModelState.IsValid)
                return View(model);


            using (var tx = session.BeginTransaction())
            {
                // synchronize available dates... probably easiest to just delete all of them and readd...
                // the GameScheduled property can be a dynamic lookup instead... if a game is already created
                // for the team on the date/time then it is simply filled
                if (team.DatesAvailable != null)
                {
                    foreach (var item in team.DatesAvailable)
                    {
                        //dont add confirmed dates
                        if (item.GetGameScheduled() != null)
                            continue;
                        session.Delete(item);
                    }
                }
                //team.DatesAvailable.Clear();

                if (model.AvailableDates != null)
                {
                    foreach (var item in model.AvailableDates)
                    {
                        var date = new AvailableDates();
                        date.Team = team;
                        date.Location = Location.GetLocationById(item.LocationId);
                        date.Date = DateTime.Parse(string.Format("{0} {1}", item.Date, item.Time));
                        // don't add dates in the past
                        if (date.Date < DateTime.Now)
                            continue;
                        date.GameScheduled = false;
                        date.IsHome = (item.Type == "Home" || item.Type == "HomeOrAway");
                        date.IsAway = (item.Type == "Away" || item.Type == "HomeOrAway");
                        if (date.IsAway)
                        {
                            date.DistanceFromLocation = item.Distance;
                        }
                        else
                        {
                            date.DistanceFromLocation = null;
                        }
                        team.DatesAvailable.Add(date);
                        session.Save(date);
                    }
                }

                tx.Commit();
            }

            TempData["TeamUpdated"] = true;
            if (Submit == "SkipPlayers")
                return RedirectToAction("Index", "Game");
            if(team.Players.Count == 0) //Send to Create Player panel if no players
                return RedirectToAction("Create", "Player", new { id = team.Id });
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            using (var tx = session.BeginTransaction())
            {               
                var item = Team.GetTeamById(id, user);
                var standings = BracketTeam.GetStandingsForTeam(item);
                if (standings.Count > 0)
                {
                    TempData["Error"] = "The team cannot be deleted because it is included in a bracket.";
                    return RedirectToAction("Index");
                }

                var games = Game.GetGamesForTeam(item);
                if (games.Count > 0)
                {
                    if (user.Role == UserRole.Administrator)
                    {
                        foreach (var game in games)
                        {
                            session.Delete(game);
                        }
                        session.Flush();
                        session.Delete(item);
                    }
                    else
                    {
                        TempData["Error"] = "The team cannot be deleted because it has associated games.";
                        return RedirectToAction("Index");
                    }
                }

                if (item != null && item.Players.Count == 0)
                {
                    foreach (var date in item.DatesAvailable)
                    {
                        session.Delete(date);
                    }
                    session.Flush();
                    session.Delete(item);
                    TempData["TeamDeleted"] = true;
                }
                else if (item.Players.Count != 0)
                {
                    TempData["Error"] = "The team cannot be deleted because it still has linked players.";
                }
                tx.Commit();
            }
            
            return RedirectToAction("Index");
        }
    }
}