using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.SqlServer.Types;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();

        private IList<SelectListItem> GetTeamsList(User user)
        {
            var items = new List<SelectListItem>();
            if (user == null)
                return items;
            var teams = Team.GetTeamsForUser(user).OrderByDescending(l => l.Name);
            foreach (var item in teams)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var divisions = session.QueryOver<Division>().List();
            ViewBag.Divisions = divisions;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Classifieds()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string postTitle = "Contact Form Submission";
            var body = new StringBuilder();
            var bodyMessage = new StringBuilder();
            body.AppendLine(string.Format("The following contact form was submitted at {0} from {1}: ", DateTime.Now, Request.ServerVariables["REMOTE_ADDR"]));
            bodyMessage.AppendLine(string.Format("{0, -10}: {1} ", "Name", model.Name));
            bodyMessage.AppendLine(string.Format("{0, -10}: {1} ", "E-mail", model.Email));
            bodyMessage.AppendLine(string.Format("{0, -10}: {1} ", "Message", model.Message));
            body.Append(bodyMessage.ToString());

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("info@iscrimmage.com"));
            msg.From = new MailAddress(model.Email);
            msg.Subject = postTitle;
            body.AppendLine();
            body.AppendLine("--");
            msg.Body = body.ToString();
            msg.IsBodyHtml = false;
            using (var client = new SmtpClient())
            {
                client.Send(msg);
            }

            TempData["ContactSuccess"] = true;
            return RedirectToAction("Contact");
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }

        public ActionResult WhyUs()
        {
            return View();
        }

        public ActionResult Coaches_Managers()
        {
            return View();
        }

        public ActionResult Players_Parents()
        {
            return View();
        }

        public ActionResult Tournaments()
        {
            return View();
        }

        private LeagueInfoModel PopulateLeagueInfo(League league, int? teamId, User user)
        {
            var model = new LeagueInfoModel();
            if (teamId.HasValue)
            {
                var team = Web.Models.Team.GetTeamById(teamId.Value);
                model.Team = team;

                model.Fees = new List<LeagueFeeModel>();

                var fees = Fee.GetFeesForLeague(league);
                var feesPaid = FeePayment.GetFeePaymentsForTeam(team);
                foreach (var fee in fees)
                {
                    var feeModel = new LeagueFeeModel();
                    feeModel.Amount = fee.Amount;
                    feeModel.Name = fee.Name;
                    feeModel.InvoiceId = string.Format("{0}_{1}", fee.Id, team.Id);
                    feeModel.IsPaid = false;

                    var feePayment = feesPaid.Where(f => f.Fee == fee && f.Status == FeePaymentStatus.Completed).SingleOrDefault();
                    if (feePayment != null)
                    {
                        feeModel.IsPaid = true;
                    }
                    model.Fees.Add(feeModel);
                }
            }            

            model.League = league;

            var games = new List<Game>();
            foreach (var team in league.Teams)
            {
                var teamGames = Game.GetGamesForTeam(team);
                games.AddRange(teamGames);
            }
            games = games.Distinct().OrderBy(g => g.GameDate).ToList();
            model.Games = games;
            
            if (user != null)
            {
                model.Teams = Web.Models.Team.GetTeamsForUser(user).Where(m => m.League == league).ToList();
            }            

            return model;
        }

        public ActionResult League(int id, int? teamId)
        {
            var league = Web.Models.League.GetLeagueById(id);
            ViewBag.LeagueType = "league";
            ViewBag.Teams = Web.Models.Team.GetAllTeams().OrderBy(m => m.Division.MaxAge).ThenBy(m => m.Class.Name).ThenBy(m => m.Name).ToList();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (user != null)
            {
                ViewBag.TeamsList = GetTeamsList(user);
            }

            var model = PopulateLeagueInfo(league, teamId, user);

            return View("Tournament", model);
        }

        // this isn't really necessary since we can map two routes to the same action
        //public ActionResult Tournament(int id, int teamId)
        //{
        //    var tournament = Web.Models.League.GetActiveTournamentById(id);
        //    ViewBag.LeagueType = "tournament";
        //    ViewBag.Teams = Web.Models.Team.GetAllTeams().OrderBy(m => m.Division.MaxAge).ThenBy(m => m.Class.Name).ThenBy(m => m.Name).ToList();
        //    var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
        //    if (user != null)
        //    {
        //        ViewBag.TeamsList = GetTeamsList(user);
        //    }

        //    var model = PopulateLeagueInfo(tournament, teamId, user);

        //    return View("Tournament", model);
        //}


        public ActionResult Tournament01(int id = 0)
        {
            int TournamentId = 4;
            var tournament = Web.Models.League.GetActiveTournamentById(TournamentId);
            ViewBag.Teams = Web.Models.Team.GetAllTeams().OrderBy(m => m.Division.MaxAge).ThenBy(m => m.Class.Name).ThenBy(m => m.Name).ToList();
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (user != null)
            {
                ViewBag.TeamsList = GetTeamsList(user);
                ViewBag.TeamsInTournament = Web.Models.Team.GetTeamsForUser(user).Where(m => m.League.Id == TournamentId).ToList();
            }

            var model = new LeagueInfoModel();

            if (id > 0)
            {
                var team = Web.Models.Team.GetTeamById(id);
                ViewBag.Team = team;
                
                model.League = tournament;
                model.Team = team;
                model.Fees = new List<LeagueFeeModel>();

                //string feeId = null;
                //bool isPaidUp = true;

                var fees = Fee.GetFeesForLeague(tournament);
                var feesPaid = FeePayment.GetFeePaymentsForTeam(team);
                foreach (var fee in fees)
                {
                    var feeModel = new LeagueFeeModel();
                    feeModel.Amount = fee.Amount;
                    feeModel.Name = fee.Name;
                    feeModel.InvoiceId = string.Format("{0}_{1}", fee.Id, team.Id);
                    feeModel.IsPaid = false;

                    var feePayment = feesPaid.Where(f=>f.Fee == fee && f.Status == FeePaymentStatus.Completed).SingleOrDefault();
                    if (feePayment != null)
                    {
                        feeModel.IsPaid = true;
                    }
                    model.Fees.Add(feeModel);
                }

                
                //var mandatoryFees = Fee.GetRequiredFeesForLeague(tournament);
                //if (mandatoryFees.Count > 0)
                //{
                //    // right now we are assuming only one mandatory fee
                //    feeId = string.Format("{0}_{1}", mandatoryFees[0].Id, team.Id);
                    
                //    var feesPaid = FeePayment.GetFeePaymentsForTeam(team);
                //    foreach (var fee in mandatoryFees)
                //    {
                //        var isFeePaid = (feesPaid.Where(f => f.Fee == fee && f.Status == FeePaymentStatus.Completed).SingleOrDefault() != null);
                //        if (!isFeePaid)
                //        {
                //            isPaidUp = false;
                //            break;
                //        }
                //    }
                //}


                // a team is registered if they have paid the mandatory fees for the tournament
                //var teamRegistration = TournamentRegistration.GetTournamentregistrationForTeam(tournament, team);
                //if (teamRegistration == null)
                //{
                //    using (var tx = session.BeginTransaction())
                //    {
                //        var registration = new TournamentRegistration();
                //        registration.Team = team;
                //        registration.Tournament = tournament;
                //        registration.RegisteredOn = null;
                //        session.Save(registration);
                //        tx.Commit();
                //        registrationId = registration.Id;
                //    }                    
                //}
                //else
                //{
                //    registrationId = teamRegistration.Id;
                //    if (teamRegistration.Payment != null)
                //        isPaidUp = true;
                //}
                //ViewBag.InvoiceId = feeId;
                //ViewBag.IsRegistered = isPaidUp;

            }
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Tournament01(BasicTournamentRegistrationModel model)
        {
            var resp = new AjaxResponse();
            resp.Success = false;
            if (!ModelState.IsValid)
                return Json(resp);

            var body = new StringBuilder();
            var msg = new MailMessage();
            msg.To.Add(new MailAddress("adrian@bltbaseball.com"));
            msg.From = new MailAddress("noreply@bltbaseball.com");
            msg.Subject = "Tournament Registration Submission";
            var sb = new StringBuilder();
            sb.AppendFormat(
@"Coach/Manager Name(s): 
{0} 
 
Coach/Manager Email: 
{1} 
 
Coach/Manager Phone Number: 
{2} 
 
Phone Carrier: 
{3}  
 
Team Name: 
{4} 
 
Age Group: 
{5} 
 
Class: 
{6} 
 
# of Seasons Played: 
{7} 
  
Date: {8} ",
           model.Name,
           model.Email,
           model.PhoneNumber,
           model.PhoneCarrier,
           model.Team,
           model.AgeGroup,
           model.Class,
           model.SeasonsPlayed,
           DateTime.Now);

            msg.Body = sb.ToString();
            msg.IsBodyHtml = false;
            using (var client = new SmtpClient())
            {
                client.Send(msg);
            }
            resp.Success = true;
            return Json(resp);

            //TempData["RegistrationSuccess"] = true;
            //return RedirectToAction("Tournament01");
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Downloads()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestInvite(InviteRequestModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            if (user.Role != UserRole.Unknown)
            {
                return RedirectToAction("Dashboard");
            }

            if (!ModelState.IsValid)
                return View("NeedInvite", model);

            switch (model.Role)
            {
                // automatically create and link up certain roles
                case UserRole.Coach:
                    using (var tx = session.BeginTransaction())
                    {
                        var item = new Web.Models.Coach();
                        item.FirstName = model.FirstName;
                        item.LastName = model.LastName;
                        item.Email = User.Identity.Name;
                        item.PhoneNumber = model.PhoneNumber;
                        item.CreatedOn = DateTime.Now;
                        item.User = user;
                        session.Save(item);

                        user.Role = UserRole.Coach;
                        session.Update(user);
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        tx.Commit();
                    }
                    if(!String.IsNullOrEmpty(model.RedirectAction))
                        return Redirect(model.RedirectAction);
                    break;
                case UserRole.Player:
                    // validate fields
                    if (string.IsNullOrEmpty(model.FirstName))
                        ModelState.AddModelError("FirstName", "You must enter a First Name.");
                    if (string.IsNullOrEmpty(model.LastName))
                        ModelState.AddModelError("LastName", "You must enter a Last Name.");
                    if (string.IsNullOrEmpty(model.PhoneNumber))
                        ModelState.AddModelError("PhoneNumber", "You must enter a Phone Number.");
                    if (!model.DateOfBirth.HasValue)
                        ModelState.AddModelError("DateOfBirth", "You must enter a valid Date Of Birth.");
                    else
                    {
                        if (model.DateOfBirth > DateTime.Now || model.DateOfBirth < (DateTime.Now.AddYears(-25)))
                            ModelState.AddModelError("DateOfBirth", "You must enter a valid Date Of Birth.");
                    }
                    if (!ModelState.IsValid)
                        return View("NeedInvite", model);
                    using (var tx = session.BeginTransaction())
                    {
                        var player = new Player();
                        //Player.GetPlayerForEmail(user.Email);
                        player.FirstName = model.FirstName;
                        player.LastName = model.LastName;
                        player.PhoneNumber = model.PhoneNumber;
                        player.DateOfBirth = model.DateOfBirth;
                        player.Email = user.Email;
                        player.Gender = model.Gender;
                        player.CreatedOn = DateTime.Now;
                        player.User = user;
                        user.Role = UserRole.Player;
                        session.Update(user);
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Save(player);
                        tx.Commit();
                    }
                    break;
                case UserRole.Guardian:
                    if (string.IsNullOrEmpty(model.FirstName))
                    {
                        ModelState.AddModelError("FirstName", "You must enter a First Name.");
                    }
                    if (string.IsNullOrEmpty(model.LastName))
                    {
                        ModelState.AddModelError("LastName", "You must enter a Last Name.");
                    }
                    if (!ModelState.IsValid)
                        return View("NeedInvite", model);

                    // unique key check
                    var existingGuardian = Guardian.GetGuardianByEmail(user.Email);
                    if (existingGuardian != null)
                    {
                        ModelState.AddModelError("", "A guardian with your e-mail address already exists.");
                        return View("NeedInvite", model);
                    }
                    using (var tx = session.BeginTransaction())
                    {
                        var guardian = new Guardian();
                        guardian.FirstName = model.FirstName;
                        guardian.LastName = model.LastName;
                        guardian.Email = user.Email;
                        guardian.User = user;
                        guardian.CreatedOn = DateTime.Now;
                        guardian.CreatedBy = user;
                        user.Role = UserRole.Guardian;
                        session.Update(user);
                        ControllerContext.HttpContext.Cache.Remove("User-" + user.Email);
                        session.Save(guardian);
                        tx.Commit();
                    }

                    break;
                default:
                    // email an administrator the request
                    var msg = new MailMessage();
                    msg.To.Add(new MailAddress("adrian@bltbaseball.com"));
                    msg.From = new MailAddress("noreply@bltbaseball.com");
                    msg.Subject = "Invitation Request";
                    var sb = new StringBuilder();
                    sb.AppendFormat(
    @"Role: {0} 
First Name: {1} 
Last Name: {2} 
E-mail: {3} 
Phone Number: {4}
User: http://{5}/Users/{6}

Date: {7} ", model.Role, model.FirstName, model.LastName, User.Identity.Name, model.PhoneNumber, System.Web.HttpContext.Current.Request.Url.Host, user.Id, DateTime.Now);
                    msg.Body = sb.ToString();
                    msg.IsBodyHtml = false;
                    using (var client = new SmtpClient())
                    {
                        client.Send(msg);
                    }
                    TempData["RequestSent"] = true;
                    break;
            }

            return RedirectToAction("Dashboard");
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult CoachInvite(int leagueId = 0)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);

            if (user.Role == UserRole.Coach)
            {
                var coach = Web.Models.Coach.GetCoachForUser(user);
                var teams = Web.Models.Team.GetTeamsWithCoach(coach);
                return Redirect("~/Team/Create/" + leagueId.ToString());
                //return Redirect("~/Home/Tournament01");
                /*if(teams.Count > 0)
                    return Redirect("~/Team");
                else
                    return Redirect("~/Team/Create");*/
            }
            var model = new InviteRequestModel();
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.UserId = user.Id;
            return View("CoachInvite", model);
        }


        [Authorize]
        public ActionResult Dashboard()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            switch(user.Role) {
                case UserRole.Coach:
                    return RedirectToAction("Coach", "Profile");
                case UserRole.Manager:
                    return RedirectToAction("Manager", "Profile");
                case UserRole.Player:
                    return RedirectToAction("Player", "Profile");
                case UserRole.Umpire:
                    return RedirectToAction("Umpire", "Profile");
                case UserRole.Guardian:
                    return RedirectToAction("Guardian", "Profile");
                case UserRole.Administrator:
                    break;
                default:
                    var model = new InviteRequestModel();
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    model.UserId = user.Id;
                    return View("NeedInvite", model);
            }

            return View();
        }

        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }
    }
}
