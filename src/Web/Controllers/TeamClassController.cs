using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class TeamClassController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        //private WebContext db = new WebContext();
        
        //
        // GET: /TeamClass/

        public ActionResult Index()
        {            
            var items = session.QueryOver<TeamClass>().List();
            return View(items);
        }

        //
        // GET: /TeamClass/Details/5

        public ActionResult Details(int id = 0)
        {
            // populate TeamClassUpdateModel instead
            var item = session.Get<TeamClass>(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // GET: /TeamClass/Create

        public ActionResult Create()
        {
            var model = new TeamClassNewModel();
            return View(model);
        }

        //
        // POST: /TeamClass/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamClassNewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var teamclass = new TeamClass();
                teamclass.Name = model.Name;
                teamclass.Handicap = model.Handicap;
                teamclass.CreatedOn = DateTime.Now;
                session.Save(teamclass);
                tx.Commit();

                TempData["TeamClassCreated"] = true;
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /TeamClass/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var model = new TeamClassUpdateModel();
            var item = session.Get<TeamClass>(id);
            if (item == null)
                return HttpNotFound();

            model.Name = item.Name;
            model.Handicap = item.Handicap;
            return View(model);
        }

        //
        // POST: /TeamClass/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamClassUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<TeamClass>(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.Name = model.Name;
                item.Handicap = model.Handicap;
                session.Update(item);
                tx.Commit();
            }

            TempData["TeamClassUpdated"] = true;
            return RedirectToAction("Index");
        }

        //
        // GET: /TeamClass/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    var item = session.Get<TeamClass>(id);
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);
        //}

        //
        // POST: /TeamClass/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var tx = session.BeginTransaction())
            {
                var item = session.Get<TeamClass>(id);
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["TeamClassDeleted"] = true;
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}