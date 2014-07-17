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
    [AuthorizeRoles(UserRole.Administrator)]
    public class ManagersController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();
        private const string IMAGE_PATH = "~/Images/Managers";

        private IList<SelectListItem> GetTeamList()
        {
            var teams = Team.GetAllTeams();
            var items = new List<SelectListItem>();
            foreach (var t in teams)
            {
                items.Add(new SelectListItem { Text = Team.PrettyName(t), Value = t.Id.ToString() });
            }
            return items;
        }

        public ActionResult Index()
        {
            var model = new ManagerListModel();
            model.Managers = Manager.GetAllManagers();
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.Teams = GetTeamList();
            var model = new ManagerNewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManagerNewModel model, HttpPostedFileBase imagePhoto)
        {
            ViewBag.Teams = GetTeamList();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
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
                    var item = new Manager();
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
                                team.Managers.Add(item);
                                session.Update(team);
                            }
                        }
                    }
                    tx.Commit();

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendManagerInvite(item, user);
                    }
                    else
                    {
                        EmailNotification.NewManager(item);
                    }
                    TempData["ManagerCreated"] = true;
                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the manager.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var manager = Manager.GetManagerById(id);
            if (manager == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendManagerInvite(manager, user);
                TempData["ManagerInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the manager.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index");            
        }

        public ActionResult Edit(int id)
        {
            var model = new ManagerUpdateModel();
            var manager = Manager.GetManagerById(id);
            if (manager == null)
                return RedirectToAction("Index");

            model.Id = manager.Id;
            model.Email = manager.Email;
            model.FirstName = manager.FirstName;
            model.LastName = manager.LastName;
            model.PhoneNumber = manager.PhoneNumber;
            if (!string.IsNullOrEmpty(manager.Photo))
            {
                string photoThumbPath = Path.Combine(IMAGE_PATH, manager.Photo + "-thumb." + manager.PhotoType);
                if (System.IO.File.Exists(Server.MapPath(IMAGE_PATH) + "/" + manager.Photo + "-thumb." + manager.PhotoType))
                {
                    model.CurrentPhotoPath = photoThumbPath;
                }
            }
            model.TeamIds = Team.GetTeamsWithManager(manager).Select(t => t.Id).ToList();
            var allTeams = GetTeamList();
            ViewBag.ManagerTeams = allTeams.Where(i => model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            ViewBag.Teams = allTeams.Where(i => !model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManagerUpdateModel model, HttpPostedFileBase imagePhoto)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = Manager.GetManagerById(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            var allTeams = GetTeamList();
            ViewBag.ManagerTeams = allTeams.Where(i => model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
            ViewBag.Teams = allTeams.Where(i => !model.TeamIds.ToList().Contains(int.Parse(i.Value))).ToList();
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

                    // update managers team listings
                    var existingTeams = Team.GetTeamsWithManager(item);
                    foreach (var team in existingTeams)
                    {
                        team.Managers.Remove(item);
                        session.Update(team);
                    }
                    if (model.TeamIds != null)
                    {
                        foreach (var id in model.TeamIds)
                        {
                            var team = Team.GetTeamById(id);
                            if (team != null)
                            {
                                team.Managers.Add(item);
                                session.Update(team);
                            }
                        }
                    }

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendManagerInvite(item, user);
                    }

                    tx.Commit();
                    TempData["ManagerUpdated"] = true;
                }
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to update the manager.");
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
                var manager = Manager.GetManagerById(id);
                if (manager != null)
                {
                    using (var tx = session.BeginTransaction())
                    {
                        // todo: just use cascade behavior
                        var teams = Team.GetTeamsWithManager(manager);
                        foreach (var t in teams)
                        {
                            t.Managers.Remove(manager);
                            session.Update(t);
                        }

                        string photoPath = Path.Combine(IMAGE_PATH, manager.Photo + "." + manager.PhotoType);
                        string photoThumbPath = Path.Combine(IMAGE_PATH, manager.Photo + "-thumb." + manager.PhotoType);
                        session.Delete(manager);

                        // delete the photos
                        if (System.IO.File.Exists(photoPath))
                            System.IO.File.Delete(photoPath);
                        if (System.IO.File.Exists(photoThumbPath))
                            System.IO.File.Delete(photoThumbPath);

                        tx.Commit();
                    }

                    TempData["ManagerDeleted"] = true;
                }

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                TempData["Error"] = "An error occurred while trying to delete the manager.";
                return RedirectToAction("Index");
            }
        }
    }
}
