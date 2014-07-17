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
    public class DivisionController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        private IList<SelectListItem> GetLeagueList()
        {
            var items = new List<SelectListItem>();
            var leagues = session.QueryOver<League>().OrderBy(l => l.CreatedOn).Desc.List();
            foreach (var item in leagues)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }
        //private WebContext db = new WebContext();
        
        //
        // GET: /Division/

        public ActionResult Index()
        {            
            var items = session.QueryOver<Division>().List();
            return View(items);
        }

        //
        // GET: /Division/Details/5

        public ActionResult Details(int id = 0)
        {
            // populate DivisionUpdateModel instead
            var item = session.Get<Division>(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // GET: /Division/Create

        public ActionResult Create()
        {
            var model = new DivisionNewModel();
            ViewBag.Leagues = GetLeagueList();
            return View(model);
        }

        //
        // POST: /Division/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DivisionNewModel model)
        {
            ViewBag.Leagues = GetLeagueList();

            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var division = new Division();
                division.Name = model.Name;
                //division.League = session.Load<League>(model.LeagueId.Value);
                division.MaxAge = model.MaxAge;
                division.CreatedOn = DateTime.Now;
                session.Save(division);
                tx.Commit();

                TempData["DivisionCreated"] = true;
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Division/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var model = new DivisionUpdateModel();
            ViewBag.Leagues = GetLeagueList();
            var item = session.Get<Division>(id);
            if (item == null)
                return HttpNotFound();

            model.Name = item.Name;
            //model.LeagueId = item.League.Id;
            model.MaxAge = item.MaxAge;
            return View(model);
        }

        //
        // POST: /Division/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DivisionUpdateModel model)
        {
            ViewBag.Leagues = GetLeagueList();
            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<Division>(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.Name = model.Name;
                //item.League = session.Load<League>(model.LeagueId);
                item.MaxAge = model.MaxAge;
                session.Update(item);
                tx.Commit();
            }

            TempData["DivisionUpdated"] = true;
            return RedirectToAction("Index");
        }

        //
        // GET: /Division/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    var item = session.Get<Division>(id);
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);
        //}

        //
        // POST: /Division/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var tx = session.BeginTransaction())
            {
                var item = session.Get<Division>(id);
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["DivisionDeleted"] = true;
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}