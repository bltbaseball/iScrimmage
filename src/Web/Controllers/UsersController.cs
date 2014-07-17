using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web.Filters;
using Web.Models;
using WebMatrix.WebData;

namespace Web.Controllers
{
    //[InitializeSimpleMembership]
    [Authorize]
    [Web.Helpers.AuthorizeRoles(UserRole.Administrator)]
    public class UsersController : Controller
    {
        //public static SelectListItem CreateSelectListItemFromEnum(object t)
        //{
        //    FieldInfo field = t.GetType().GetField(t.ToString());
        //    DisplayAttribute attrs = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), false).First();
        //    return new SelectListItem() { Value = t.ToString(), Text = attrs.GetName() };
        //}

        private IList<SelectListItem> GetRoleList()
        {
            var items = new List<SelectListItem>();
            foreach (var t in Enum.GetValues(typeof(UserRole)))
            {
                items.Add(new SelectListItem { Text = t.ToString(), Value = t.ToString() });
            }
            return items;
        }

        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        public ActionResult Index()
        {
            ViewBag.Users = Web.Models.User.GetAllUsers();
            return View();
        }

        public ActionResult Create()
        {
            var model = new UserNewModel();
            ViewBag.Roles = GetRoleList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserNewModel model)
        {
            ViewBag.Roles = GetRoleList();
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var existingUser = Web.Models.User.GetUserByEmail(model.Email);
                    if (existingUser != null)
                        ModelState.AddModelError("Email", "A user with that e-mail address already exists.");
                }

                if (!ModelState.IsValid)
                    return View(model);

                using (var tx = session.BeginTransaction())
                {
                    var user = new User();
                    user.Email = model.Email;
                    user.Role = model.Role;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.CreateDate = DateTime.Now;
                    session.Save(user);
                    tx.Commit();
                    // todo: send invitation to user based on their role?

                    ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                }

                TempData["UserCreated"] = true;
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the user.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Roles = GetRoleList();
            var model = new UserUpdateModel();
            var user = Web.Models.User.GetUserById(id);
            if (user == null)
                return RedirectToAction("Index");

            model.Email = user.Email;
            model.Role = user.Role;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Id = user.Id;
            ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserUpdateModel model)
        {
            ViewBag.Roles = GetRoleList();
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = Web.Models.User.GetUserById(model.Id);
                if (user == null)
                    return RedirectToAction("Index");

                if (!model.Email.ToLowerInvariant().Equals(user.Email.ToLowerInvariant()))
                {
                    var otherUser = Web.Models.User.GetUserByEmail(model.Email);
                    if (otherUser != null)
                        ModelState.AddModelError("Email", "That email address is already in use.");
                }

                // if the email address is changed, we also need to unlink the google authentication as it will no longer match...

                if (!ModelState.IsValid)
                    return View(model);

                ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                using (var tx = session.BeginTransaction())
                {
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Role = model.Role;
                    session.Update(user);
                    tx.Commit();
                    TempData["UserUpdated"] = true;
                }
                ControllerContext.HttpContext.Cache.Remove("User-" + model.Email);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to update the user.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var user = Web.Models.User.GetUserById(id);
                if (user != null)
                {

                    switch (user.Role)
                    {
                        case UserRole.Player:
                            var player = Player.GetPlayerForUser(user);
                            if (player != null)
                            {
                                player.User = null;
                                session.Update(player);
                            }
                            break;
                        case UserRole.Coach:
                            var coach = Coach.GetCoachForUser(user);
                            if (coach != null)
                            {
                                coach.User = null;
                                session.Update(coach);
                            }
                            break;
                        case UserRole.Guardian:
                            var guardian = Guardian.GetGuardianForUser(user);
                            if (guardian != null)
                            {
                                guardian.User = null;
                                session.Update(guardian);
                            }
                            break;
                        case UserRole.Manager:
                            var manager = Manager.GetManagerForUser(user);
                            if (manager != null)
                            {
                                manager.User = null;
                                session.Update(manager);
                            }
                            break;
                        case UserRole.Umpire:
                            var umpire = Umpire.GetUmpireForUser(user);
                            if (umpire != null)
                            {
                                umpire.User = null;
                                session.Update(umpire);
                            }
                            break;
                    }
                    Web.Models.User.DeleteUser(user);
                    ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                    TempData["UserDeleted"] = true;
                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                TempData["Error"] = "An error occurred while trying to delete the user.";
                return RedirectToAction("Index");
            }
        }
    }
}
