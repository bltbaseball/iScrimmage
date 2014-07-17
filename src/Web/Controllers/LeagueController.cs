using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class LeagueController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        private IList<SelectListItem> GetLeagueTypes()
        {
            var items = new List<SelectListItem>();
            foreach (var t in Enum.GetValues(typeof(LeagueType)))
            {
                items.Add(new SelectListItem { Text = t.ToString(), Value = t.ToString() });
            }
            return items;
        }

        //
        // GET: /League/

        public ActionResult Index()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var leagues = session.QueryOver<League>().List();
            return View(leagues);
        }

        //
        // GET: /League/Details/5

        public ActionResult Details(int id)
        {
            var item = session.Get<League>(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        //
        // GET: /League/Create

        public ActionResult Create()
        {
            var model = new LeagueNewModel();
            model.Fees = new List<FeeNewModel>();
            model.MinimumDatesAvailable = 15;
            ViewBag.LeagueTypes = GetLeagueTypes();
            return View(model);
        }

        //
        // POST: /League/Create

        [HttpPost]
        public ActionResult Create(LeagueNewModel model)
        {
            ViewBag.LeagueTypes = GetLeagueTypes();
            DateTime registrationStartDate, startDate, endDate, registrationEndDate;
            DateTime? rosterLockedOn = null;

            if (!ModelState.IsValid)
                return View(model);

            if (!DateTime.TryParse(model.RegistrationStartDate, out registrationStartDate))
                ModelState.AddModelError("RegistrationStartDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.RegistrationEndDate, out registrationEndDate))
                ModelState.AddModelError("RegistrationEndDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.StartDate, out startDate))
                ModelState.AddModelError("StartDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.EndDate, out endDate))
                ModelState.AddModelError("EndDate", "Please enter a valid date.");

            if (!string.IsNullOrEmpty(model.RosterLockedOn))
            {
                DateTime rosterDate;
                if (!DateTime.TryParse(model.RosterLockedOn, out rosterDate))
                {
                    ModelState.AddModelError("RosterLockedOn", "Please enter a valid date.");
                }
                else
                {
                    rosterLockedOn = rosterDate;
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            // update db from model
            using (var tx = session.BeginTransaction())
            {
                var league = new League();
                league.Name = model.Name;
                league.Url = model.Url;
                league.StartDate = startDate;
                league.EndDate = endDate;
                league.RegistrationStartDate = registrationStartDate;
                league.RegistrationEndDate = registrationEndDate;
                league.RosterLockedOn = rosterLockedOn;
                league.IsActive = model.IsActive;
                league.WaiverRequired = model.WaiverRequired;
                league.CreatedOn = DateTime.Now;
                league.Type = model.Type;
                league.HtmlDescription = model.HtmlDescription;
                if (league.Type == LeagueType.League)
                {
                    league.MinimumDatesAvailable = model.MinimumDatesAvailable;
                }
                else
                {
                    league.MinimumDatesAvailable = null;
                }
                session.Save(league);

                if (model.Fees != null)
                {
                    foreach (var modelFee in model.Fees)
                    {
                        var fee = new Fee();
                        fee.League = league;
                        fee.Amount = modelFee.Amount;
                        fee.Name = modelFee.Name;
                        fee.IsRequired = modelFee.IsRequired;
                        fee.CreatedOn = DateTime.Now;
                        session.Save(fee);
                    }
                }

                tx.Commit();

                TempData["LeagueCreated"] = true;
                return RedirectToAction("Index");
            } 
        }

        //
        // GET: /League/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.LeagueTypes = GetLeagueTypes();
            var model = new LeagueUpdateModel();
            var item = session.Get<League>(id);
            if (item == null)
                return HttpNotFound();
            model.Name = item.Name;
            model.Url = item.Url;
            model.StartDate = item.StartDate.ToShortDateString();
            model.EndDate = item.EndDate.ToShortDateString();
            model.RegistrationStartDate = item.RegistrationStartDate.ToShortDateString();
            model.RegistrationEndDate = item.RegistrationEndDate.ToShortDateString();
            model.RosterLockedOn = item.RosterLockedOn.HasValue ? item.RosterLockedOn.Value.ToShortDateString() : null;
            model.HtmlDescription = item.HtmlDescription;
            model.IsActive = item.IsActive;
            model.WaiverRequired = item.WaiverRequired;
            model.Type = item.Type;
            model.MinimumDatesAvailable = item.MinimumDatesAvailable;
            var fees = Fee.GetFeesForLeague(item);
            var modelFees = new List<FeeEditModel>();
            foreach (var fee in fees)
            {
                var modelFee = new FeeEditModel();
                modelFee.Id = fee.Id;
                modelFee.Amount = fee.Amount;
                modelFee.IsRequired = fee.IsRequired;
                modelFee.Name = fee.Name;
                modelFees.Add(modelFee);
            }
            model.Fees = modelFees;

            return View(model);
        }

        //
        // POST: /League/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeagueUpdateModel model)
        {
            ViewBag.LeagueTypes = GetLeagueTypes();

            if (!ModelState.IsValid)
                return View(model);

            DateTime registrationStartDate, startDate, endDate, registrationEndDate;
            DateTime? rosterLockedOn = null;

            if (!ModelState.IsValid)
                return View(model);

            if (!DateTime.TryParse(model.RegistrationStartDate, out registrationStartDate))
                ModelState.AddModelError("RegistrationStartDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.RegistrationEndDate, out registrationEndDate))
                ModelState.AddModelError("RegistrationEndDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.StartDate, out startDate))
                ModelState.AddModelError("StartDate", "Please enter a valid date.");

            if (!DateTime.TryParse(model.EndDate, out endDate))
                ModelState.AddModelError("EndDate", "Please enter a valid date.");

            if (!string.IsNullOrEmpty(model.RosterLockedOn))
            {
                DateTime rosterDate;
                if (!DateTime.TryParse(model.RosterLockedOn, out rosterDate))
                {
                    ModelState.AddModelError("RosterLockedOn", "Please enter a valid date.");
                }
                else
                {
                    rosterLockedOn = rosterDate;
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var item = session.Get<League>(model.Id);
            if (item == null)
                return RedirectToAction("Index");

            using (var tx = session.BeginTransaction())
            {
                item.Name = model.Name;
                item.Url = model.Url;
                item.StartDate = startDate;
                item.EndDate = endDate;
                item.RegistrationStartDate = registrationStartDate;
                item.RegistrationEndDate = registrationEndDate;
                item.RosterLockedOn = rosterLockedOn;
                item.IsActive = model.IsActive;
                item.WaiverRequired = model.WaiverRequired;
                item.Type = model.Type;
                item.HtmlDescription = model.HtmlDescription;
                if (item.Type == LeagueType.League)
                {
                    item.MinimumDatesAvailable = model.MinimumDatesAvailable;
                }
                else
                {
                    item.MinimumDatesAvailable = null;
                }
                session.Update(item);

                var existingFees = Fee.GetFeesForLeague(item);
                if (model.Fees == null)
                {
                    foreach (var fee in existingFees)
                    {
                        // todo: fees probably shouldnt really be deleted if there have been associated payments
                        // but merely flagged as deleted
                        session.Delete(fee);
                    }
                }
                else
                {
                    var updatedFeeIds = model.Fees.Where(f => f.Id != null).Select(f => f.Id.Value).Distinct().ToList();
                    var existingFeeIds = existingFees.Select(f => f.Id).ToList();
                    var removedFees = existingFeeIds.Except(updatedFeeIds);
                    var newFees = model.Fees.Where(f => f.Id == null);
                    foreach (var feeId in removedFees)
                    {
                        var fee = Fee.GetFeeById(feeId);
                        session.Delete(fee);
                    }

                    foreach (var updatedFee in model.Fees)
                    {
                        if (updatedFee.Id.HasValue)
                        {
                            var fee = Fee.GetFeeById(updatedFee.Id.Value);
                            fee.Amount = updatedFee.Amount;
                            fee.IsRequired = updatedFee.IsRequired;
                            fee.Name = updatedFee.Name;
                            session.Update(fee);
                        }
                        else
                        {
                            var fee = new Fee();
                            fee.League = item;
                            fee.Amount = updatedFee.Amount;
                            fee.Name = updatedFee.Name;
                            fee.IsRequired = updatedFee.IsRequired;
                            fee.CreatedOn = DateTime.Now;
                            session.Save(fee);
                        }
                    }
                }

                tx.Commit();
            }

            TempData["LeagueUpdated"] = true;
            return RedirectToAction("Index");
        }

        //
        // POST: /League/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRole.Administrator)]
        public ActionResult Delete(int id)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var item = League.GetLeagueById(id);

            using (var tx = session.BeginTransaction())
            {
                if (item != null)
                {
                    session.Delete(item);
                }
                tx.Commit();
            }
            TempData["LeagueDeleted"] = true;
            return RedirectToAction("Index");
        }
    }
}
