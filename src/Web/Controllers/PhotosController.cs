using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        [Authorize]
        [HttpPost]
        public ActionResult CropPhoto(string File, int Top, int Left, int Bottom, int Right, int Id, string Type)
        {
            var resp = new AjaxResponse();
            resp.Success = false;
            Elmah.ErrorSignal.FromCurrentContext().Raise(new ApplicationException(string.Format("{0} - {1} - {2} - {3} - {4}", File, Top, Left, Bottom, Right)));

            var currentPhoto = File;
            var image = new WebImage("~/Images/Temp/" + File + ".jpeg");
            var height = image.Height;
            var width = image.Width;

            int bottom = image.Height - Bottom;
            if (bottom < 0)
                bottom = 0;
            int right = image.Width - Right;
            if (right > image.Width)
                right = image.Width;
            image.Crop(Top, Left, bottom, (int)(image.Width - Right));
            image.Resize(250, 250, true, false);

            image.Save(Path.Combine("~/Images/Temp", File));

            // move the photo based on the type, check user permissions for photo editing of submitted id/type
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            switch (Type)
            {
                case "Player":
                    var teamplayer = session.Get<TeamPlayer>(Id); // todo: permissions check here
                    if (teamplayer != null)
                    {                        
                        // delete existing photo, if any
                        if (!string.IsNullOrEmpty(teamplayer.Photo))
                        {
                            var photoPath = Server.MapPath(Path.Combine("~/PlayerImages", teamplayer.Photo) + ".jpeg");
                            if (System.IO.File.Exists(photoPath))
                            {
                                System.IO.File.Delete(photoPath);
                            }
                        }

                        // save new photo
                        image.Save(Path.Combine("~/PlayerImages", File));
                        teamplayer.Photo  = File;
                        using(var tx = session.BeginTransaction()) 
                        {
                            session.Update(teamplayer);
                            tx.Commit();
                        }
                    }
                    break;
                case "Coach":
                    var coach = Coach.GetCoachById(Id, user);
                    if (coach != null)
                    {
                        // delete existing photo, if any
                        if (!string.IsNullOrEmpty(coach.Photo))
                        {
                            var photoPath = Server.MapPath(Path.Combine("~/Images/Coaches", coach.Photo) + ".jpeg");
                            if (System.IO.File.Exists(photoPath))
                            {
                                System.IO.File.Delete(photoPath);
                            }
                        }

                        // save new photo
                        image.Save(Path.Combine("~/Images/Coaches", File));
                        coach.Photo = File;
                        using (var tx = session.BeginTransaction())
                        {
                            session.Update(coach);
                            tx.Commit();
                        }
                    }
                    break;
                case "Manager":
                    var manager = Manager.GetManagerById(Id, user);
                    if (manager != null)
                    {
                        // delete existing photo, if any
                        if (!string.IsNullOrEmpty(manager.Photo))
                        {
                            var photoPath = Server.MapPath(Path.Combine("~/Images/Managers", manager.Photo) + ".jpeg");
                            if (System.IO.File.Exists(photoPath))
                            {
                                System.IO.File.Delete(photoPath);
                            }
                        }

                        // save new photo
                        image.Save(Path.Combine("~/Images/Managers", File));
                        manager.Photo = File;
                        using (var tx = session.BeginTransaction())
                        {
                            session.Update(manager);
                            tx.Commit();
                        }
                    }
                    break;
                case "Umpire":
                    var umpire = Umpire.GetUmpireById(Id, user);
                    if (umpire != null)
                    {
                        // delete existing photo, if any
                        if (!string.IsNullOrEmpty(umpire.Photo))
                        {
                            var photoPath = Server.MapPath(Path.Combine("~/Images/Umpires", umpire.Photo) + ".jpeg");
                            if (System.IO.File.Exists(photoPath))
                            {
                                System.IO.File.Delete(photoPath);
                            }
                        }

                        // save new photo
                        image.Save(Path.Combine("~/Images/Umpires", File));
                        umpire.Photo = File;
                        using (var tx = session.BeginTransaction())
                        {
                            session.Update(umpire);
                            tx.Commit();
                        }
                    }
                    break;
            }

            resp.Success = true;
            return Json(resp);
        }

        [HttpPost]
        public ActionResult UploadPhoto(HttpPostedFileBase Photo)
        {
            try
            {                
                if (Photo == null)
                {
                    return Content("empty");
                }

                int length = Photo.ContentLength;
                if (length == 0)
                {
                    return Content("empty");
                }

                string extension = string.Empty;
                switch (Photo.ContentType)
                {
                    case "image/pjpeg":
                    case "image/jpeg":
                    case "image/jpg":
                        extension = "jpeg";
                        break;
                    default:
                        return Content("{\"success\": false, \"error\": \"InvalidFileType\", \"errorDescription\":\"" + Photo.ContentType + "\"}");
                }

                if (Photo != null)
                {
                    string photo = Guid.NewGuid().ToString().Replace("-", "");

                    var image = new WebImage(Photo.InputStream);

                    if (image != null)
                    {
                        if (image.Width > 500)
                        {
                            image.Resize(500, ((500 * image.Height) / image.Width));
                        }

                        var filename = Path.GetFileName(image.FileName);
                        image.Save(Path.Combine("~/Images/Temp", photo));

                        return Content("{\"success\": true, \"type\":\"" + extension + "\", \"file\": \"" + photo + "\"}");
                    }
                }

                return Content("{\"success\": false, \"error\": \"Generic\", \"errorDescription\":\"An error occurred while uploading the photo.\"}");
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content("empty");
            }
        }

    }
}
