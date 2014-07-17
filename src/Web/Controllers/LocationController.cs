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
    public class LocationController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        //private WebContext db = new WebContext();
        
        //
        // GET: /Location/

        public ActionResult Index()
        {            
            var items = session.QueryOver<Location>().List();
            return View(items);
        }

        //
        // GET: /Location/Details/5

        public ActionResult Details(int id = 0)
        {
            // populate LocationUpdateModel instead
            var item = session.Get<Location>(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        //
        // GET: /Location/Create

        public ActionResult Create()
        {
            var model = new LocationNewModel();
            return View(model);
        }

        //
        // POST: /Location/Create

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(LocationNewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_CreateLocation", model);
                }
                else
                {
                    return View(model);
                }
            }

            GeoCoding.Microsoft.BingMapsGeoCoder geoCoder = new GeoCoding.Microsoft.BingMapsGeoCoder(System.Configuration.ConfigurationManager.AppSettings["BingMapsApiKey"]);

            var addresses = geoCoder.GeoCode(model.Address, model.City, model.State, model.Zip, "US");

            if (addresses.ToList().Count == 0)
            {
                TempData["LocationLookupFailed"] = true;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_CreateLocation", model);
                }
                else
                {
                    return View(model);
                }
            }

            var address = addresses.FirstOrDefault();

            if (address.Confidence != GeoCoding.Microsoft.ConfidenceLevel.High)
            {
                TempData["LocationLookupFailed"] = true;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_CreateLocation", model);
                }
                else
                {
                    return View(model);
                }
            }

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var location = new Location();
                location.Name = model.Name;
                location.Address = model.Address;
                location.City = model.City;
                location.State = model.State;
                location.Zip = model.Zip;
                location.Url = model.Url;
                location.Notes = model.Notes;
                location.GroundsKeeperPhone = model.GroundsKeeperPhone;
                location.Latitude = address.Coordinates.Latitude;
                location.Longitude = address.Coordinates.Longitude;
                //location.Point = new NetTopologySuite.Geometries.Point(location.Longitude, location.Latitude);
                //location.Point.SRID = 4326;
                location.CreatedOn = DateTime.Now;
                session.Save(location);
                tx.Commit();

                if (Request.IsAjaxRequest())
                {
                    var ret = new LocationCreateResult();
                    ret.Success = true;
                    ret.Locations = Location.GetAllLocations();
                    ret.LocationId = location.Id;
                    return Json(ret);
                }
                else
                {
                    TempData["LocationCreated"] = true;
                    return RedirectToAction("Index");
                }
            }
        }
        //
        // GET: /Location/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var model = new LocationUpdateModel();
            var item = session.Get<Location>(id);
            if (item == null)
                return HttpNotFound();

            model.Name = item.Name;
            model.Address = item.Address;
            model.City = item.City;
            model.State = item.State;
            model.Zip = item.Zip;
            model.Url = item.Url;
            model.Notes = item.Notes;
            model.GroundsKeeperPhone = item.GroundsKeeperPhone;

            return View(model);
        }

        //
        // POST: /Location/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LocationUpdateModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<Location>(model.Id);
            if (item == null)
                return RedirectToAction("Index");


            GeoCoding.Microsoft.BingMapsGeoCoder geoCoder = new GeoCoding.Microsoft.BingMapsGeoCoder(System.Configuration.ConfigurationManager.AppSettings["BingMapsApiKey"]);

            var addresses = geoCoder.GeoCode(model.Address, model.City, model.State, model.Zip, "US");

            if (addresses.ToList().Count == 0)
            {
                TempData["LocationLookupFailed"] = true;
                return View(model);
            }

            var address = addresses.FirstOrDefault();

            if (address.Confidence != GeoCoding.Microsoft.ConfidenceLevel.High ||
                address.Type == GeoCoding.Microsoft.EntityType.RoadBlock)
            {
                TempData["LocationLookupFailed"] = true;
                return View(model);
            }

            using (var tx = session.BeginTransaction())
            {
                item.Name = model.Name;
                item.Address = model.Address;
                item.City = model.City;
                item.State = model.State;
                item.Zip = model.Zip;
                item.Url = model.Url;
                item.Notes = model.Notes;
                item.Latitude = address.Coordinates.Latitude;
                item.Longitude = address.Coordinates.Longitude;
                //item.Point = new NetTopologySuite.Geometries.Point(item.Longitude, item.Latitude);
                //item.Point.SRID = 4326;
                item.GroundsKeeperPhone = model.GroundsKeeperPhone;
                session.Update(item);
                tx.Commit();
            }

            TempData["LocationUpdated"] = true;
            return RedirectToAction("Index");
        }

        //
        // GET: /Location/Delete/5

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
        // POST: /Location/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var tx = session.BeginTransaction())
            {
                var item = session.Get<Location>(id);
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["LocationDeleted"] = true;
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}