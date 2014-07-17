using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class GuardianController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        //private WebContext db = new WebContext();

        //
        // GET: /Guardian/

        public ActionResult Index()
        {
            var items = session.QueryOver<Guardian>().List();
            return View(items);
        }

        //
        // GET: /Guardian/Details/5

        public ActionResult Details(int id = 0)
        {
            // populate LocationUpdateModel instead
            var item = session.Get<Guardian>(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // GET: /Guardian/Create

        public ActionResult Create()
        {
            var model = new GuardianNewModel();
            return View(model);
        }

        //
        // POST: /Guardian/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GuardianNewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var guardian = new Guardian();
                guardian.FirstName = model.FirstName;
                guardian.LastName = model.LastName;
                guardian.Address = model.Address;
                guardian.City = model.City;
                guardian.State = model.State;
                guardian.Zip = model.Zip;
                guardian.PhoneNumber = model.PhoneNumber;
                guardian.Email = model.Email;
                guardian.CreatedOn = DateTime.Now;
                session.Save(guardian);
                tx.Commit();

                EmailNotification.NewGuardian(guardian);
                TempData["GuardianCreated"] = true;
                return RedirectToAction("Index");
            }
        }
        //
        // GET: /Guardian/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var model = new GuardianUpdateModel();
            var item = session.Get<Guardian>(id);
            if (item == null)
                return HttpNotFound();

            model.FirstName = item.FirstName;
            model.LastName = item.LastName;
            model.Address = item.Address;
            model.City = item.City;
            model.State = item.State;
            model.Zip = item.Zip;
            model.PhoneNumber = item.PhoneNumber;
            model.Email = item.Email;
            
            return View(model);
        }

        //
        // POST: /Guardian/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GuardianUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<Guardian>(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.FirstName = model.FirstName;
                item.LastName = model.LastName;
                item.Address = model.Address;
                item.City = model.City;
                item.State = model.State;
                item.Zip = model.Zip;
                item.PhoneNumber = model.PhoneNumber;
                item.Email = model.Email;
                session.Update(item);
                tx.Commit();
            }

            TempData["GuardianUpdated"] = true;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var guardian = Guardian.GetGuardianById(id);
            if (guardian == null)
                return RedirectToAction("Index");
            try
            {
                Helpers.Invite.SendGuardianInvite(guardian, null, user);
                TempData["GuardianInvited"] = true;
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to invite the guardian.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Guardian/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    var item = session.Get<Location>(id);
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);
        //}

        //
        // POST: /Guardian/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var tx = session.BeginTransaction())
            {
                var item = session.Get<Guardian>(id);
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["GuardianDeleted"] = true;
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
