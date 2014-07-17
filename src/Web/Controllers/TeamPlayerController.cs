using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using System.IO;
using System.Web.Helpers;
using NHibernate.Criterion;
using Web.Helpers;
using System.Threading;
using System.Configuration;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
    public class TeamPlayerController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        private IList<SelectListItem> GetTeamsList(User user)
        {
            var items = new List<SelectListItem>();
            var teams = Team.GetTeamsForUser(user).OrderByDescending(l => l.Name);
            foreach (var item in teams)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        private IList<SelectListItem> GetPlayersList(User user)
        {
            var items = new List<SelectListItem>();
            var players = Player.GetPlayersForUser(user).OrderByDescending(l=>l.LastName).ThenByDescending(l=>l.FirstName).ToList();
            foreach (var item in players)
            {
                items.Add(new SelectListItem { Text = item.LastName + ", " + item.FirstName, Value = item.Id.ToString() });
            }
            return items;
        }

        //public ActionResult Test()
        //{
        //    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
        //    string teamId = "1";
        //    var team = Team.GetTeamById(int.Parse(teamId), user);
        //    if (team == null)
        //        return RedirectToAction("Index", "Team");
        //    var teamMaxAge = team.Division.MaxAge;
        //    var cutoffdate = PlayerHelper.PlayerCutoffDate(teamMaxAge);
        //    var searchList = Player.GetPlayersForUser(user);
        //    Response.Write(searchList.Count() + "<br />");
        //    var free = Player.GetFreeAgentPlayers();
        //    Response.Write("Free:" + free.Count() + "<br />");
        //    string term = "free";
        //    var filteredList = searchList.Where(p => p.DateOfBirth > cutoffdate && (p.FirstName.ToLowerInvariant().StartsWith(term.ToLowerInvariant()) || p.LastName.ToLowerInvariant().StartsWith(term.ToLowerInvariant())));
        //    Response.Write(filteredList.Count() + "<br />");
        //    return View();
        //}

        public ActionResult SearchPlayers(string teamId, string term)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Team.GetTeamById(int.Parse(teamId), user);
            if (team == null)
                return RedirectToAction("Index", "Team");

            var newPlayerArray = new
            {
                id = 0,
                label = "Create New Player",
                email = "",
                guardianEmail = "",
                guardianId = 0,
                lastName = "",
                firstName = "",
                dateOfBirth = "",
                name = "PlayerId"
            };
            var teamMaxAge = team.Division.MaxAge;
            var cutoffdate = PlayerHelper.PlayerCutoffDate(teamMaxAge);
            //Temporary until system is full of players
            IList<Player> playerPool;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["RestrictPlayerSearch"]))
            {
                playerPool = Player.GetPlayersForUser(user);
            }
            else
            {
                playerPool = Player.GetAllPlayers();
            }
            var searchList = Player.GetAllPlayers()
                .Where(p => p.DateOfBirth > cutoffdate && (
                    (p.Email == null ? false : p.Email.ToLower().Contains(term.ToLower())) ||
                    p.FirstName.ToLower().Contains(term.ToLower()) ||
                    p.LastName.ToLower().Contains(term.ToLower()) ||
                    (p.LastName + " " + p.FirstName).ToLower().Contains(term.ToLower()) ||
                    (p.FirstName + " " + p.LastName).ToLower().Contains(term.ToLower())
                    )
                )
                .Select(r => new { id = r.Id, 
                    label = r.LastName + ", " + r.FirstName + " " + (r.DateOfBirth.HasValue ? PlayerHelper.PlayerAge(r.DateOfBirth.Value) + "U " + r.DateOfBirth.Value.ToShortDateString() : ""),
                                   email = r.Email,
                                   guardianEmail = r.Guardian == null ? "" : r.Guardian.Email,
                                   guardianId = r.Guardian == null ? 0 : r.Guardian.Id,
                                   lastName = r.LastName,
                                   firstName = r.FirstName,
                                   dateOfBirth = (r.DateOfBirth.HasValue ? r.DateOfBirth.Value.ToShortDateString() : ""),
                                   name = "PlayerId"
                });
            var list = new List<object>();
            list.Add(newPlayerArray);
            list.AddRange(searchList.ToList());
            if (searchList.Count() > 0)
                return Json(list, JsonRequestBehavior.AllowGet);
            else {
                List<object> myList=new List<object>();
                myList.Add(newPlayerArray);
                return Json(myList, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult SearchGuardians(string term)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var newGuardianArray = new
            {
                id = 0,
                label = "Create New Guardian",
                email = "",
                name = "GuardianId"
            };

            var searchList = Guardian.GetAllGuardians().Where<Guardian>(p => (
                    (p.Email == null ? false : p.Email.ToLower().Contains(term.ToLower())) ||
                    (p.FirstName == null ? false : p.FirstName.ToLower().Contains(term.ToLower())) ||
                    (p.LastName == null ? false : p.LastName.ToLower().Contains(term.ToLower()))
                    )
                ).Select(r => new { id = r.Id, label = r.FirstName + " " + r.LastName + " " + r.Email, email = r.Email, name = "GuardianId" });
            
            var list = new List<object>();
            list.Add(newGuardianArray);
            if(searchList != null)
                list.AddRange(searchList.ToList());
            if (searchList.Count() > 0)
                return Json(list, JsonRequestBehavior.AllowGet);
            else
            {
                List<object> myList = new List<object>();
                myList.Add(newGuardianArray);
                return Json(myList, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /TeamPlayer/

        public ActionResult Index(int id = 0)
        {
            var model = new TeamPlayersModel();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Team.GetTeamById(id, user);
            if(team == null)
                return RedirectToAction("Index", "Team");
            
            model.Team = team;
            model.AvailableDates = AvailableDatesDTO.MapAvailableDates(team.DatesAvailable.Where(d => d.Date >= DateTime.Now).ToList());
            var players = team.Players.OrderBy(x => x.Player.LastName);
            model.Players = TeamPlayerDTO.MapTeamPlayers(players.ToList());
            model.Status = Team.GetTeamStatusForTeam(team);
            
            return View(model);
        }
        
        //
        // GET: /TeamPlayer/Create

        public ActionResult Create(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (id == 0)
                throw new InvalidOperationException("No Team ID");
            var model = new TeamPlayerNewModel();
            ViewBag.Players = GetPlayersList(user);
            ViewBag.Team = Team.GetTeamById(id, user); ;
            return View(model);
        }

        //
        // POST: /TeamPlayer/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamPlayerNewModel model, HttpPostedFileBase imageFile)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Team.GetTeamById(model.TeamId);
            if (team == null)
                return RedirectToAction("Index", "Team");

            ViewBag.Players = GetPlayersList(user);
            ViewBag.Team = team;
            
            if (!ModelState.IsValid)
                return View(model);
            
            var player = Player.GetPlayerById(model.PlayerId);
            if (player == null)
            {
                ModelState.AddModelError("PlayerId", "You must choose a valid player.");
            }

            if (imageFile == null)
            {
                ModelState.AddModelError("", "You must choose a photo to upload.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var image = new WebImage(imageFile.InputStream);

            if (image != null)
            {
                if (image.Width > 500)
                {
                    image.Resize(500, ((500 * image.Height) / image.Width));
                }

                model.Photo = Guid.NewGuid().ToString().Replace("-", "");

                var filename = Path.GetFileName(image.FileName);
                image.Save(Path.Combine("~/PlayerImages", model.Photo));
            }

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var teamplayer = new TeamPlayer();
                teamplayer.Player = player;
                teamplayer.Team = team;
                teamplayer.IsPhotoVerified = false;
                teamplayer.Photo = model.Photo;
                teamplayer.Status = model.Status;
                teamplayer.CreatedOn = DateTime.Now;
                session.Save(teamplayer);
                tx.Commit();

                EmailNotification.PlayerAddedToTeam(teamplayer);
                TempData["TeamPlayerCreated"] = true;
                return RedirectToAction("Index", new { id = model.TeamId });
            }
        }

        //
        // GET: /TeamPlayer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var model = new TeamPlayerUpdateModel();
            ViewBag.Players = GetPlayersList(user);

            var item = session.Get<TeamPlayer>(id);
            if (item == null)
                return HttpNotFound();

            ViewBag.Team = item.Team;

            var player = item.Player;
            ViewBag.PlayerName = player.FirstName + ", " + player.LastName + (player.DateOfBirth.HasValue ? " " + player.DateOfBirth.Value.ToShortDateString() : "");

            model.PlayerId = item.Player.Id;
            model.TeamId = item.Team.Id;
            model.IsPhotoVerified = item.IsPhotoVerified;
            model.Photo = item.Photo;
            model.Status = item.Status;
           
            return View(model);
        }

        //
        // POST: /TeamPlayer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamPlayerUpdateModel model, HttpPostedFileBase imageFile)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Players = GetPlayersList(user);

            var item = session.Get<TeamPlayer>(model.Id);
            if (item == null)
                return HttpNotFound();

            ViewBag.Team = item.Team;

            if (!ModelState.IsValid)
                return View(model);

            var team = Team.GetTeamById(model.TeamId, user);
            if (team == null)
                return RedirectToAction("Index", "Team");

            var player = Player.GetPlayerById(model.PlayerId);
            if (player == null)
            {
                ModelState.AddModelError("PlayerId", "You must choose a valid player.");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (imageFile != null)
            {
                var image = new WebImage(imageFile.InputStream);

                if (image != null)
                {
                    if (image.Width > 500)
                    {
                        image.Resize(500, ((500 * image.Height) / image.Width));
                    }

                    model.Photo = Guid.NewGuid().ToString().Replace("-", "");

                    var filename = Path.GetFileName(image.FileName);
                    image.Save(Path.Combine("~/PlayerImages", model.Photo));
                }
            }

            if (item == null)
                return RedirectToAction("Index", new { id = model.TeamId });
            
            ViewBag.Team = item.Team;
            
            using (var tx = session.BeginTransaction())
            {
                item.Player = player;
                item.Team = team;
                item.IsPhotoVerified = model.IsPhotoVerified;
                if (imageFile != null)
                    item.Photo = model.Photo;
                item.Status = model.Status;
                session.Update(item);
                tx.Commit();
            }

            TempData["TeamPlayerUpdated"] = true;
            return RedirectToAction("Index", new { id = model.TeamId });
        }

        //
        // GET: /TeamPlayer/Photo/5

        public ActionResult Photo(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            var model = new TeamPlayerUpdateModel();

            var item = session.Get<TeamPlayer>(id);
            if (item == null)
                return HttpNotFound();

            ViewBag.Team = item.Team;

            var player = item.Player;
            ViewBag.PlayerName = player.FirstName + ", " + player.LastName + (player.DateOfBirth.HasValue ? " " + player.DateOfBirth.Value.ToShortDateString() : "");

            model.PlayerId = item.Player.Id;
            model.TeamId = item.Team.Id;
            model.IsPhotoVerified = item.IsPhotoVerified;
            model.Photo = item.Photo;
            model.Status = item.Status;

            return View(model);
        }

        public ActionResult Test()
        {
            return View();
        }

        //
        // POST: /TeamPlayer/Photo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPhoto(TeamPlayerUpdateModel model, HttpPostedFileBase imageFile)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            if (imageFile != null)
            {
                var image = new WebImage(imageFile.InputStream);

                if (image != null)
                {
                    if (image.Width > 500)
                    {
                        image.Resize(500, ((500 * image.Height) / image.Width));
                    }

                    model.Photo = Guid.NewGuid().ToString().Replace("-", "");

                    var filename = Path.GetFileName(image.FileName);
                    image.Save(Path.Combine("~/PlayerImages", model.Photo));
                }
            }

            var item = session.Get<TeamPlayer>(model.Id);
            if (item == null)
                return RedirectToAction("Index", new { id = model.TeamId });

            ViewBag.Team = item.Team;

            using (var tx = session.BeginTransaction())
            {
                if (imageFile != null)
                    item.Photo = model.Photo;
                session.Update(item);
                tx.Commit();
            }

            TempData["PhotoUpdated"] = true;
            return View("Photo", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CropImage(TeamPlayerUpdateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Players = GetPlayersList(user);

            var item = session.Get<TeamPlayer>(model.Id);
            if (item == null)
                return RedirectToAction("Index", new { id = model.TeamId });

            ViewBag.Team = item.Team;
            
            var currentPhoto = item.Photo;
            var image = new WebImage("~/PlayerImages/" + item.Photo + ".jpeg");
            var height = image.Height;
            var width = image.Width;
            image.Crop((int)model.Top, (int)model.Left, (int)(image.Height - model.Bottom), (int)(image.Width - model.Right));
            image.Resize(250, 250, true, false);
            using (var tx = session.BeginTransaction())
            {
                item.Photo = Guid.NewGuid().ToString().Replace("-", "");
                model.Photo = item.Photo;
                session.Update(item);
                tx.Commit();
            }
            image.Save(Path.Combine("~/PlayerImages", item.Photo));

            System.IO.File.Delete(Server.MapPath(Path.Combine("~/PlayerImages", currentPhoto) + ".jpeg"));
            return View("Photo", model);
        }

        public ActionResult Delete(int id)
        {
            int teamId = 0;
            using (var tx = session.BeginTransaction())
            {
                var item = session.Get<TeamPlayer>(id);
                if (item != null)
                {
                    teamId = item.Team.Id;
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["TeamPlayerDeleted"] = true;
            return RedirectToAction("Index", new { id = teamId });
        }
        
        public ActionResult SendWaiver(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = session.Get<TeamPlayer>(id);
            if (item == null)
                return HttpNotFound();

            var team = Team.GetTeamById(item.Team.Id, user);
            var player = team.Players.Where(i => i.Player.Id == item.Player.Id).Select(i => i.Player).Single();

            var RequestId = Hellosign.RequestPlayerWaiverSignature(session, item, player, team);

            return RedirectToAction("Index", new { id = team.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id, int teamId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var player = Player.GetPlayerById(id, user);
            if (player == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendPlayerInvite(player, user);
                TempData["PlayerInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the player.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index", new { id = teamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteGuardian(int id, int playerId, int teamId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var guardian = Guardian.GetGuardianById(id);
            var player = Player.GetPlayerById(playerId, user);
            if (player == null)
                return RedirectToAction("Index");
            if (guardian == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendGuardianInvite(guardian, player, user);
                TempData["GuardianInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the guardian.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index", new { id = teamId });
        }
    }
}