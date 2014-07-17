using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class InvitationController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        public ActionResult Manager(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
                return View("InviteError");

            var manager = Web.Models.Manager.GetManagerWithInviteToken(token);
            if (manager == null)
            {
                return View("InviteError");
            }

            // if user is not authenticated, request that they login to the site
            if (User.Identity.IsAuthenticated)
            {
                using (var tx = session.BeginTransaction())
                {
                    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                    if ((manager.User != null) && (manager.User != user))
                    {
                        return View("InviteClaimed");
                    }
                    else if (manager.User == user)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (user != null)
                    {
                        // check that the user is not already associated
                        if (user.Role != UserRole.Unknown)
                        {
                            return View("InviteExistingAccount");
                        }
                        manager.User = user;
                        user.Role = UserRole.Manager;
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Update(manager);
                        tx.Commit();
                    }
                }

                // redirect to dashboard
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (manager.User != null)
                {
                    return View("InviteClaimed");
                }

                var model = new InviteManagerModel();
                model.FirstName = manager.FirstName;
                model.LastName = manager.LastName;
                model.Email = manager.Email;
                ViewBag.ReturnUrl = Url.Action("Manager", "Invitation", new { id = id, token = token });
                return View(model);
            }
        }

        public ActionResult Player(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
                return View("InviteError");

            var player = Web.Models.Player.GetPlayerWithInviteToken(token);
            if (player == null)
            {
                return View("InviteError");
            }

            // if user is not authenticated, request that they login to the site
            if (User.Identity.IsAuthenticated)
            {
                using (var tx = session.BeginTransaction())
                {
                    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

                    if ((player.User != null) && (player.User != user))
                    {
                        return View("InviteClaimed");
                    }
                    else if (player.User == user)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (user != null)
                    {
                        // check that the user is not already associated
                        if (user.Role != UserRole.Unknown)
                        {
                            return View("InviteExistingAccount");
                        }
                        player.User = user;
                        user.Role = UserRole.Player;
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Update(player);
                        tx.Commit();
                    }
                }

                // redirect to dashboard
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (player.User != null)
                {
                    return View("InviteClaimed");
                }

                var model = new InvitePlayerModel();
                model.FirstName = player.FirstName;
                model.LastName = player.LastName;
                model.Email = player.Email;
                ViewBag.ReturnUrl = Url.Action("Player", "Invitation", new { id = id, token = token });
                return View(model);
            }
        }


        public ActionResult Guardian(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
                return View("InviteError");

            var guardian = Web.Models.Guardian.GetGuardianWithInviteToken(token);
            if (guardian == null)
            {
                return View("InviteError");
            }

            // if user is not authenticated, request that they login to the site
            if (User.Identity.IsAuthenticated)
            {
                using (var tx = session.BeginTransaction())
                {
                    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                    if ((guardian.User != null) && (guardian.User != user))
                    {
                        return View("InviteClaimed");
                    }
                    else if (guardian.User == user)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (user != null)
                    {
                        // check that the user is not already associated
                        if (user.Role != UserRole.Unknown)
                        {
                            return View("InviteExistingAccount");
                        }
                        guardian.User = user;
                        user.Role = UserRole.Guardian;
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Update(guardian);
                        tx.Commit();
                    }
                }

                // redirect to dashboard
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (guardian.User != null)
                {
                    return View("InviteClaimed");
                }

                var model = new InviteGuardianModel();
                model.FirstName = guardian.FirstName;
                model.LastName = guardian.LastName;
                model.Email = guardian.Email;
                ViewBag.ReturnUrl = Url.Action("Guardian", "Invitation", new { id = id, token = token });
                return View(model);
            }
        }

        public ActionResult Coach(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
                return View("InviteError");

            var coach = Web.Models.Coach.GetCoachWithInviteToken(token);
            if (coach == null)
            {
                return View("InviteError");
            }

            // if user is not authenticated, request that they login to the site
            if (User.Identity.IsAuthenticated)
            {
                using (var tx = session.BeginTransaction())
                {
                    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                    if ((coach.User != null) && (coach.User != user))
                    {
                        return View("InviteClaimed");
                    }
                    else if (coach.User == user)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (user != null)
                    {
                        // check that the user is not already associated
                        if (user.Role != UserRole.Unknown)
                        {
                            return View("InviteExistingAccount");
                        }

                        coach.User = user;
                        user.Role = UserRole.Coach;
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Update(coach);
                        tx.Commit();
                    }
                }

                // redirect to dashboard
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (coach.User != null)
                {
                    return View("InviteClaimed");
                }

                var model = new InviteCoachModel();
                model.FirstName = coach.FirstName;
                model.LastName = coach.LastName;
                model.Email = coach.Email;
                ViewBag.ReturnUrl = Url.Action("Coach", "Invitation", new { id = id, token = token });
                return View(model);
            }
        }

        public ActionResult Umpire(int id, string token)
        {
            if (string.IsNullOrEmpty(token))
                return View("InviteError");

            var item = Web.Models.Umpire.GetUmpireWithInviteToken(token);
            if (item == null)
            {
                return View("InviteError");
            }

            // if user is not authenticated, request that they login to the site
            if (User.Identity.IsAuthenticated)
            {
                using (var tx = session.BeginTransaction())
                {
                    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                    if ((item.User != null) && (item.User != user))
                    {
                        return View("InviteClaimed");
                    }
                    else if (item.User == user)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (user != null)
                    {
                        // check that the user is not already associated
                        if (user.Role != UserRole.Unknown)
                        {
                            return View("InviteExistingAccount");
                        }

                        item.User = user;
                        user.Role = UserRole.Umpire;
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Update(item);
                        tx.Commit();
                    }
                }

                // redirect to dashboard
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                if (item.User != null)
                {
                    return View("InviteClaimed");
                }

                var model = new InviteUmpireModel();
                model.FirstName = item.FirstName;
                model.LastName = item.LastName;
                model.Email = item.Email;
                ViewBag.ReturnUrl = Url.Action("Umpire", "Invitation", new { id = id, token = token });
                return View(model);
            }
        }


        public ActionResult GenerateInvite()
        {
            var model = new GenerateInviteCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateInvite(GenerateInviteCreateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            if (!ModelState.IsValid)
                return View(model);


            //check if user already exists
            if (Web.Models.User.GetUserByEmail(model.Email) != null)
            {
                TempData["Error"] = "User already exists in the sytem.";
                return View(model);
            }

            //remove the person if they are not a user and re-invite them
            var manager = Web.Models.Manager.GetManagerForEmail(model.Email);
            var coach = Web.Models.Coach.GetCoachForEmail(model.Email);
            var guardian = Web.Models.Guardian.GetGuardianForEmail(model.Email);
            var player = Web.Models.Player.GetPlayerForEmail(model.Email);
            
            if (manager != null)
                session.Delete(manager);
            if (coach != null)
                session.Delete(coach);
            if (guardian != null)
                session.Delete(guardian);
            if (player != null)
            {
                if(player.Teams.Count() > 0)
                {
                    TempData["Error"] = "Player is already assigned to a team. Cannot re-invite.";
                    return View(model);
                }
                session.Delete(player);
            }

            var recipients = new List<string>();
            recipients.Add(model.Email);

            try
            {
                switch (model.MessageTo)
                {
                    case "CoachChallenge":
                        using (var tx = session.BeginTransaction())
                        {
                            var item = new Coach();
                            item.FirstName = model.FirstName;
                            item.LastName = model.LastName;
                            item.Email = model.Email;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = user;
                            item.InviteToken = Guid.NewGuid().ToString();
                            item.InvitationSentOn = DateTime.Now;
                            session.Save(item);
                            tx.Commit();
                            EmailNotification.InviteCoachChallenge(item, user);
                        }

                        break;
                    case "Coach":
                        using (var tx = session.BeginTransaction())
                        {
                            var item = new Coach();
                            item.FirstName = model.FirstName;
                            item.LastName = model.LastName;
                            item.Email = model.Email;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = user;
                            item.InviteToken = Guid.NewGuid().ToString();
                            item.InvitationSentOn = DateTime.Now;
                            session.Save(item);
                            tx.Commit();
                            EmailNotification.InviteCoach(item, user);
                        }
                        break;
                    case "Guardian":
                        using (var tx = session.BeginTransaction())
                        {
                            var item = new Guardian();
                            item.FirstName = model.FirstName;
                            item.LastName = model.LastName;
                            item.Email = model.Email;
                            item.CreatedOn = DateTime.Now;
                            item.CreatedBy = user;
                            item.InviteToken = Guid.NewGuid().ToString();
                            item.InvitationSentOn = DateTime.Now;
                            session.Save(item);
                            tx.Commit();
                            EmailNotification.InviteGuardian(item, user);
                        }
                        break;
                    case "Player":
                        using (var tx = session.BeginTransaction())
                        {
                            var item = new Player();
                            item.FirstName = model.FirstName;
                            item.LastName = model.LastName;
                            item.Email = model.Email;
                            item.CreatedOn = DateTime.Now;
                            item.InviteToken = Guid.NewGuid().ToString();
                            item.InvitationSentOn = DateTime.Now;
                            session.Save(item);
                            tx.Commit();
                            EmailNotification.InvitePlayer(item, user);
                        }
                        break;

                }
                TempData["MessageSent"] = true;
                return RedirectToAction("GenerateInvite");
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to send the message.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }
    }
}
