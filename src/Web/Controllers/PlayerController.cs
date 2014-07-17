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
using Web.Helpers;
using System.Configuration;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager, UserRole.Player, UserRole.Guardian)]
    public class PlayerController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        //
        // GET: /Player/

        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager, UserRole.Guardian)]
        public ActionResult Index()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if(user.Role == UserRole.Guardian)
                return RedirectToAction("Dashboard", "Home");
            var items = Player.GetPlayersForUser(user);
            return View(items);
        }
        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
        public ActionResult Create(int id = 0, int updatePlayerId = 0)
        {
            var model = new PlayerNewModel();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (id == 0)
                throw new InvalidOperationException("No Team ID");
            ViewBag.Team = Team.GetTeamById(id, user);
            model.TeamId = id;

            if (updatePlayerId > 0)
            {
                var item = Player.GetPlayerById(updatePlayerId, user);
                if (item == null)
                    return HttpNotFound();
                model.UpdatePlayerId = updatePlayerId;
                model.FirstName = item.FirstName;
                model.LastName = item.LastName;
                model.JerseyNumber = Convert.ToInt32(item.JerseyNumber);
                model.PhoneNumber = item.PhoneNumber;
                model.DateOfBirth = item.DateOfBirth;
                model.Email = item.Email;
                model.Gender = item.Gender;

                if (item.Guardian != null)
                {
                    model.Guardian = new GuardianUpdateModel();
                    model.UpdateGuardianId = item.Guardian.Id;
                    model.Guardian.Id = item.Guardian.Id;
                    model.Guardian.FirstName = item.Guardian.FirstName;
                    model.Guardian.LastName = item.Guardian.LastName;
                    model.Guardian.Email = item.Guardian.Email;
                }
            }
            return View(model);
        }
        /*public ActionResult Create()
        {
            var model = new PlayerNewModel();
            model.DateOfBirth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return View(model);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
        public ActionResult Create(PlayerNewModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var team = Team.GetTeamById((int)model.TeamId, user);
            if (team == null)
                return RedirectToAction("Index", "TeamPlayer", new { id = model.TeamId });

            ViewBag.Team = team;
            if (!ModelState.IsValid)
                return View(model);

            var commitData = false;
            if (Request.Form["Commit"] != null)
                commitData = true;

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                Player player;
                if (model.PlayerId.HasValue ? (model.PlayerId > 0 ? true : false) : false)
                {
                    // Temporary until system is full of players
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["RestrictPlayerSearch"]))
                    {
                        player = Player.GetPlayerById(model.PlayerId.Value, user);
                    }
                    else
                    {
                        player = Player.GetPlayerById(model.PlayerId.Value);
                    }
                }
                else
                {
                    if (commitData)
                    {
                        if (string.IsNullOrEmpty(model.FirstName))
                            ModelState.AddModelError("FirstName", "You must enter a Player First Name.");
                        if (string.IsNullOrEmpty(model.LastName))
                            ModelState.AddModelError("LastName", "You must enter a Player Last Name.");
                        if (string.IsNullOrEmpty(model.PhoneNumber))
                            ModelState.AddModelError("PhoneNumber", "You must enter a Player Phone Number.");

                        if (!model.DateOfBirth.HasValue)
                            ModelState.AddModelError("DateOfBirth", "You must enter a valid Player Date Of Birth.");
                        else if (PlayerHelper.PlayerAge(model.DateOfBirth.Value) > team.Division.MaxAge)
                            ModelState.AddModelError("DateOfBirth", "Player age is " + PlayerHelper.PlayerAge(model.DateOfBirth.Value).ToString() + " which is too old for team age division of " + team.Division.MaxAge.ToString());
                        else if (model.DateOfBirth > DateTime.Now || model.DateOfBirth < (DateTime.Now.AddYears(-25)))
                            ModelState.AddModelError("DateOfBirth", "You must enter a valid Player Date Of Birth.");

                        if (!ModelState.IsValid)
                            return View(model);
                    }
                    if (model.UpdatePlayerId > 0)
                        player = Player.GetPlayerById((int)model.UpdatePlayerId, user);
                    else
                        player = new Player();
                    player.FirstName = model.FirstName;
                    player.LastName = model.LastName;
                    player.JerseyNumber = model.JerseyNumber.ToString();
                    player.PhoneNumber = model.PhoneNumber;
                    player.DateOfBirth = model.DateOfBirth;
                    player.Email = model.Email;
                    player.Gender = model.Gender;
                    player.CreatedOn = DateTime.Now;
                    session.SaveOrUpdate(player);
                }

                if (model.GuardianId.HasValue ? (model.GuardianId > 0 ? true : false) : false)
                {
                    var guardian = Guardian.GetGuardianById(model.GuardianId.Value);
                    player.Guardian = guardian;
                    session.Update(player);
                }
                else
                {
                    if (commitData)
                    {
                        if (string.IsNullOrEmpty(model.Guardian.Email))
                        {
                            ModelState.AddModelError("Guardian.Email", "You must enter a Guardian Email.");
                        }
                        if (string.IsNullOrEmpty(model.Guardian.FirstName))
                        {
                            ModelState.AddModelError("Guardian.FirstName", "You must enter a Guardian First Name.");
                        }
                        if (string.IsNullOrEmpty(model.Guardian.LastName))
                        {
                            ModelState.AddModelError("Guardian.LastName", "You must enter a Guardian Last Name.");
                        }
                        if (!ModelState.IsValid)
                            return View(model);
                    }
                    if (!string.IsNullOrEmpty(model.Guardian.FirstName)
                        || !string.IsNullOrEmpty(model.Guardian.LastName)
                        || !string.IsNullOrEmpty(model.Guardian.Email))
                    {
                        Guardian guardian;
                        if (model.UpdateGuardianId > 0)
                        {
                            guardian = Guardian.GetGuardianById((int)model.UpdateGuardianId);
                        }
                        else
                        {
                            guardian = new Guardian();
                        }

                        if (!string.IsNullOrEmpty(model.Guardian.Email))
                        {
                            // unique key check
                            var existingGuardian = Guardian.GetGuardianByEmail(model.Guardian.Email);
                            if (existingGuardian != null)
                            {
                                if (existingGuardian != guardian)
                                {
                                    ModelState.AddModelError("Guardian.Email", "A guardian with this e-mail address already exists. Please choose them from the search box.");
                                    return View(model);
                                }
                            }
                        }
                        guardian.FirstName = model.Guardian.FirstName;
                        guardian.LastName = model.Guardian.LastName;
                        guardian.Email = model.Guardian.Email;
                        guardian.CreatedOn = DateTime.Now;
                        guardian.CreatedBy = user;
                        session.SaveOrUpdate(guardian);
                        //session.Flush();
                        player.Guardian = guardian;

                        session.Update(player);
                    }
                }

                TeamPlayer teamplayer;
                if (model.UpdatePlayerId > 0)
                {
                    teamplayer = TeamPlayer.GetTeamPlayerWithTeamAndPlayer(team, player);
                }
                else
                {
                    //Lookup teamplayer record based on playerid and teamid 
                    teamplayer = new TeamPlayer();
                    teamplayer.Player = player;
                    teamplayer.Team = team;
                    teamplayer.IsPhotoVerified = false;
                }
                teamplayer.JerseyNumber = player.JerseyNumber;
                if (commitData)
                {
                    teamplayer.Status = PlayerStatus.Active;
                    EmailNotification.PlayerAddedToTeam(teamplayer);
                } 
                else
                    teamplayer.Status = PlayerStatus.Inactive;
                teamplayer.CreatedOn = DateTime.Now;
                session.SaveOrUpdate(teamplayer);

                if (team.League.WaiverRequired && commitData)
                    Hellosign.RequestPlayerWaiverSignature(session, teamplayer, player, team);
                else
                    tx.Commit();

                if (model.UpdatePlayerId == 0)
                {
                    EmailNotification.NewPlayer(player);
                }

                TempData["PlayerCreated"] = true;
                return RedirectToAction("Index", "TeamPlayer", new { id = model.TeamId });
            }
        }

        public ActionResult CreateUnattached()
        {
            var model = new PlayerNewUnattachedModel();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUnattached(PlayerNewUnattachedModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                Player player;
                if (model.DateOfBirth > DateTime.Now || model.DateOfBirth < (DateTime.Now.AddYears(-25)))
                    ModelState.AddModelError("DateOfBirth", "You must enter a valid Player Date Of Birth.");
                if (!ModelState.IsValid)
                    return View(model);
                player = new Player();
                player.FirstName = model.FirstName;
                player.LastName = model.LastName;
                player.JerseyNumber = model.JerseyNumber.ToString();
                player.PhoneNumber = model.PhoneNumber;
                player.DateOfBirth = model.DateOfBirth;
                player.Email = model.Email;
                player.Gender = model.Gender;
                player.CreatedOn = DateTime.Now;
                if (user.Role == UserRole.Guardian)
                    player.Guardian = Guardian.GetGuardianForUser(user);
                session.SaveOrUpdate(player);

                EmailNotification.NewFreeAgentPlayer(player);
                tx.Commit();
            }

            TempData["PlayerCreated"] = true;
            return RedirectToAction("Dashboard", "Home");
        }
        //
        // GET: /Player/Edit/5

        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
        public ActionResult Edit(int id = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = Player.GetPlayerById(id, user);
            if (item == null)
                return HttpNotFound();
            var model = new PlayerUpdateModel();
            model.FirstName = item.FirstName;
            model.LastName = item.LastName;
            model.JerseyNumber = Convert.ToInt32(item.JerseyNumber);
            model.PhoneNumber = item.PhoneNumber;
            model.DateOfBirth = item.DateOfBirth;
            model.Email = item.Email;
            model.Gender = item.Gender;

            if (item.Guardian != null)
            {
                model.Guardian = new GuardianUpdateModel();
                model.Guardian.Id = item.Guardian.Id;
                model.Guardian.FirstName = item.Guardian.FirstName;
                model.Guardian.LastName = item.Guardian.LastName;
                model.Guardian.Email = item.Guardian.Email;
            }

            return View(model);
        }

        //
        // POST: /Player/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager)]
        public ActionResult Edit(PlayerUpdateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = Player.GetPlayerById(model.Id, user);

            if (item == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            using (var tx = session.BeginTransaction())
            {
                item.FirstName = model.FirstName;
                item.LastName = model.LastName;
                item.JerseyNumber = model.JerseyNumber.ToString();
                item.PhoneNumber = model.PhoneNumber;
                item.DateOfBirth = model.DateOfBirth;
                item.Email = model.Email;
                item.Gender = model.Gender;
                session.Update(item);

                if (model.GuardianId.HasValue)
                {
                    var guardian = Guardian.GetGuardianById(model.GuardianId.Value);
                    item.Guardian = guardian;
                    session.Update(item);
                }
                else
                {
                    if (model.Guardian.Id.HasValue)
                    {
                        // update existing guardian information
                        var guardian = Guardian.GetGuardianById(model.Guardian.Id.Value);
                        guardian.FirstName = model.Guardian.FirstName;
                        guardian.LastName = model.Guardian.LastName;
                        guardian.Email = model.Guardian.Email;
                        guardian.CreatedOn = DateTime.Now;
                        guardian.CreatedBy = user;
                        session.Save(guardian);
                    }
                    else
                    {
                        // enter new guardian information
                        if (!string.IsNullOrEmpty(model.Guardian.Email))
                        {
                            var guardian = new Guardian();
                            guardian.FirstName = model.Guardian.FirstName;
                            guardian.LastName = model.Guardian.LastName;
                            guardian.Email = model.Guardian.Email;
                            guardian.CreatedOn = DateTime.Now;
                            guardian.CreatedBy = user;
                            session.Save(guardian);
                            item.Guardian = guardian;

                            session.Update(item);
                        }
                    }
                }

                tx.Commit();
            }

            TempData["PlayerUpdated"] = true;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id)
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InviteGuardian(int id, int playerId)
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager, UserRole.Guardian)]
        public ActionResult Delete(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = Player.GetPlayerById(id, user);

            using (var tx = session.BeginTransaction())
            {
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["PlayerDeleted"] = true;
            return RedirectToAction("Index");
        }
    }
}