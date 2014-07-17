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
    public class UmpiresController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();
        private const string IMAGE_PATH = "~/Images/Umpires";

        public IList<SelectListItem> GetLeagueList(string selected = "")
        {
            var items = new List<SelectListItem>();
            var leagues = session.QueryOver<League>().OrderBy(l => l.CreatedOn).Desc.List();
            items.Add(new SelectListItem { Text = "None", Value = "0", Selected = ("0" == selected ? true : false) });
            foreach (var item in leagues)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }


        public ActionResult Index()
        {
            var model = new UmpireListModel();
            model.Umpires = Umpire.GetAllUmpires();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new UmpireNewModel();
            ViewBag.Leagues = GetLeagueList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UmpireNewModel model, HttpPostedFileBase imagePhoto)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            ViewBag.Leagues = GetLeagueList();
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
                    var item = new Umpire();
                    item.FirstName = model.FirstName;
                    item.LastName = model.LastName;
                    item.Email = model.Email;
                    item.Photo = model.Photo;
                    item.PhoneNumber = model.PhoneNumber;
                    item.PhotoType = imageType;
                    item.CreatedOn = DateTime.Now;
                    item.CreatedBy = user;
                    if (model.LeagueId.HasValue)
                        item.League = League.GetLeagueById(model.LeagueId.Value);
                    session.Save(item);

                    tx.Commit();

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendUmpireInvite(item);
                    }
                    TempData["UmpireCreated"] = true;
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the umpire.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id)
        {
            var item = Umpire.GetUmpireById(id);
            if (item == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendUmpireInvite(item);
                TempData["UmpireInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the umpire.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = new UmpireUpdateModel();
            var item = Umpire.GetUmpireById(id);
            if (item == null)
                return RedirectToAction("Index");

            ViewBag.Leagues = GetLeagueList();
            
            model.Id = item.Id;
            model.Email = item.Email;
            model.FirstName = item.FirstName;
            model.LastName = item.LastName;
            model.PhoneNumber = item.PhoneNumber;
            if (item.League != null)
                model.LeagueId = item.League.Id;
            if (!string.IsNullOrEmpty(item.Photo))
            {
                string photoThumbPath = Path.Combine(IMAGE_PATH, item.Photo + "-thumb." + item.PhotoType);
                if (System.IO.File.Exists(Server.MapPath(IMAGE_PATH) + "/" + item.Photo + "-thumb." + item.PhotoType))
                {
                    model.CurrentPhotoPath = photoThumbPath;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UmpireUpdateModel model, HttpPostedFileBase imagePhoto)
        {
            var item = Umpire.GetUmpireById(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            ViewBag.Leagues = GetLeagueList();

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
                    if (model.LeagueId.HasValue)
                        item.League = League.GetLeagueById(model.LeagueId.Value);
                    
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

                    if (model.InviteUser)
                    {
                        // perform the invitation process
                        Helpers.Invite.SendUmpireInvite(item);
                    }

                    tx.Commit();
                    TempData["UmpireUpdated"] = true;
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "An error occurred while trying to update the umpire.");
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
                var item = Umpire.GetUmpireById(id);
                if (item != null)
                {
                    using (var tx = session.BeginTransaction())
                    {
                        string photoPath = Path.Combine(IMAGE_PATH, item.Photo + "." + item.PhotoType);
                        string photoThumbPath = Path.Combine(IMAGE_PATH, item.Photo + "-thumb." + item.PhotoType);
                        session.Delete(item);

                        // delete the photos
                        if (System.IO.File.Exists(photoPath))
                            System.IO.File.Delete(photoPath);
                        if (System.IO.File.Exists(photoThumbPath))
                            System.IO.File.Delete(photoThumbPath);

                        tx.Commit();
                    }

                    TempData["UmpireDeleted"] = true;
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                TempData["Error"] = "An error occurred while trying to delete the umpire.";
                return RedirectToAction("Index");
            }
        }
    }
}
