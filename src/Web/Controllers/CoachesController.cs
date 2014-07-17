using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator, UserRole.Manager, UserRole.Coach)]
    public class CoachesController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();
        private const string IMAGE_PATH = "~/Images/Coaches";

        private IList<SelectListItem> GetTeamList(User user)
        {
            var teams = Team.GetTeamsForUser(user);
            var items = new List<SelectListItem>();
            foreach (var t in teams)
            {
                items.Add(new SelectListItem { Text = Team.PrettyName(t), Value = t.Id.ToString() });
            }
            return items;
        }

        public ActionResult Index()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new CoachListModel();
            model.Coaches = Coach.GetCoachesForUser(user);
            return View(model);
        }
        
        public ActionResult Create()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Teams = GetTeamList(user);
            var model = new CoachNewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CoachNewModel model, HttpPostedFileBase imagePhoto)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Teams = GetTeamList(user);
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (model.TeamIds.Count() == 0)
                {
                    ModelState.AddModelError("TeamIds", "Please select a team for the coach to manage.");
                    return View(model);
                }

                string imageType = null;
                if (imagePhoto != null)
                {
                    var image = new WebImage(imagePhoto.InputStream);
                    if (image != null)
                    {
                        if (image.Width > 500)
                        {
                            image.Resize(500, ((500 * image.Height) / image.Width));
                        }

                        model.Photo = Guid.NewGuid().ToString().Replace("-", "");

                        var filename = Path.GetFileName(image.FileName);
                        image.Save(Path.Combine(IMAGE_PATH, model.Photo));

                        image.Resize(100, ((100 * image.Height) / image.Width));
                        image.Save(Path.Combine(IMAGE_PATH, model.Photo + "-thumb"));
                        imageType = image.ImageFormat;
                    }
                }

                using (var tx = session.BeginTransaction())
                {
                    var item = new Coach();
                    item.FirstName = model.FirstName;
                    item.LastName = model.LastName;
                    item.Email = model.Email;
                    item.Photo = model.Photo;
                    item.PhoneNumber = model.PhoneNumber;
                    item.PhotoType = imageType;
                    item.CreatedOn = DateTime.Now;
                    item.CreatedBy = user;
                    session.Save(item);

                    if (model.TeamIds != null)
                    {
                        foreach (var id in model.TeamIds)
                        {
                            var team = Team.GetTeamById(id);
                            if (team != null)
                            {
                                team.Coaches.Add(item);
                                session.Update(team);
                            }
                        }
                    }
                    tx.Commit();

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendCoachInvite(item, user);
                    }
                    else
                    {
                        EmailNotification.NewCoach(item);
                    }
                    TempData["CoachCreated"] = true;
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the coach.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var coach = Coach.GetCoachById(id, user);
            // verify coach to user
            if (coach == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendCoachInvite(coach, user);
                TempData["CoachInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the coach.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new CoachUpdateModel();
            var coach = Coach.GetCoachById(id, user);
            if (coach == null)
                return RedirectToAction("Index");

            model.Id = coach.Id;
            model.Email = coach.Email;
            model.FirstName = coach.FirstName;
            model.LastName = coach.LastName;
            model.PhoneNumber = coach.PhoneNumber;
            if (!string.IsNullOrEmpty(coach.Photo))
            {
                string photoThumbPath = Path.Combine(IMAGE_PATH, coach.Photo + "-thumb." + coach.PhotoType);
                if (System.IO.File.Exists(Server.MapPath(IMAGE_PATH) + "/" + coach.Photo + "-thumb." + coach.PhotoType))
                {
                    model.CurrentPhotoPath = photoThumbPath;
                }
            }
            model.TeamIds = Team.GetTeamsWithCoach(coach).Select(t => t.Id).ToList();
            var allTeams = GetTeamList(user);
            ViewBag.CoachTeams = allTeams.Where(i => model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            ViewBag.Teams = allTeams.Where(i => !model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CoachUpdateModel model, HttpPostedFileBase imagePhoto)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = Coach.GetCoachById(model.Id, user);
            if (item == null)
                return RedirectToAction("Index");

            var allTeams = GetTeamList(user);
            if (model.TeamIds != null)
            {
                ViewBag.CoachTeams = allTeams.Where(i => model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
                ViewBag.Teams = allTeams.Where(i => !model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            } else
                ViewBag.Teams = allTeams.ToList();
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                string imageType = null;
                if (imagePhoto != null)
                {
                    var image = new WebImage(imagePhoto.InputStream);
                    if (image != null)
                    {
                        if (image.Width > 500)
                        {
                            image.Resize(500, ((500 * image.Height) / image.Width));
                        }

                        model.Photo = Guid.NewGuid().ToString().Replace("-", "");

                        var filename = Path.GetFileName(image.FileName);
                        image.Save(Path.Combine(IMAGE_PATH, model.Photo));

                        image.Resize(100, ((100 * image.Height) / image.Width));
                        image.Save(Path.Combine(IMAGE_PATH, model.Photo + "-thumb"));
                        imageType = image.ImageFormat;
                    }
                }

                using (var tx = session.BeginTransaction())
                {
                    item.FirstName = model.FirstName;
                    item.LastName = model.LastName;
                    item.Email = model.Email;
                    item.PhoneNumber = model.PhoneNumber;

                    if (!string.IsNullOrEmpty(model.Photo))
                    {
                        if (!string.IsNullOrEmpty(item.Photo))
                        {
                            // delete current photos
                            string photoPath = Path.Combine(IMAGE_PATH, item.Photo + "." + item.PhotoType);
                            string photoThumbPath = Path.Combine(IMAGE_PATH, item.Photo + "-thumb." + item.PhotoType);
                            if (System.IO.File.Exists(photoPath))
                                System.IO.File.Delete(photoPath);
                            if (System.IO.File.Exists(photoThumbPath))
                                System.IO.File.Delete(photoThumbPath);
                        }

                        // update with new photos
                        item.Photo = model.Photo;
                        item.PhotoType = imageType;
                    }

                    session.Update(item);

                    // update coach team listings
                    var existingTeams = Team.GetTeamsWithCoach(item);
                    foreach (var team in existingTeams)
                    {
                        team.Coaches.Remove(item);
                        session.Update(team);
                    }
                    if (model.TeamIds != null)
                    {
                        foreach (var id in model.TeamIds)
                        {
                            var team = Team.GetTeamById(id);
                            if (team != null)
                            {
                                team.Coaches.Add(item);
                                session.Update(team);
                            }
                        }
                    }

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendCoachInvite(item, user);
                    }

                    tx.Commit();
                    TempData["CoachUpdated"] = true;
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to update the coach.");
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
                var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
                var coach = Coach.GetCoachById(id, user);
                if (coach != null)
                {
                    using (var tx = session.BeginTransaction())
                    {
                        // todo: just use cascade behavior
                        var teams = Team.GetTeamsWithCoach(coach);
                        foreach (var t in teams)
                        {
                            t.Coaches.Remove(coach);
                            session.Update(t);
                        }

                        string photoPath = Path.Combine(IMAGE_PATH, coach.Photo + "." + coach.PhotoType);
                        string photoThumbPath = Path.Combine(IMAGE_PATH, coach.Photo + "-thumb." + coach.PhotoType);
                        session.Delete(coach);

                        // delete the photos
                        if (System.IO.File.Exists(photoPath))
                            System.IO.File.Delete(photoPath);
                        if (System.IO.File.Exists(photoThumbPath))
                            System.IO.File.Delete(photoThumbPath);

                        tx.Commit();
                    }

                    TempData["CoachDeleted"] = true;
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                TempData["Error"] = "An error occurred while trying to delete the coach.";
                return RedirectToAction("Index");
            }
        }
    }
}
