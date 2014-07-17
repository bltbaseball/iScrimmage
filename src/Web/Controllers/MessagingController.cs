using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    [AuthorizeRoles(UserRole.Administrator, UserRole.Coach, UserRole.Manager, UserRole.Player, UserRole.Guardian)]
    public class MessagingController : Controller
    {
        private NHibernate.ISession session = MvcApplication.SessionFactory.GetCurrentSession();
        public IList<SelectListItem> GetLeagueList(IList<League> leagues)
        {
            var items = new List<SelectListItem>();
            if (leagues == null)
                return items;
            foreach (var item in leagues)
            {
                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return items;
        }

        public IList<SelectListItem> GetTeamList(IList<Team> teams)
        {
            var items = new List<SelectListItem>();
            if (teams == null)
                return items;
            foreach (var item in teams.OrderBy(t => Team.PrettyName(t)).ToList())
            {
                items.Add(new SelectListItem { Text = Team.PrettyName(item), Value = item.Id.ToString() });
            }
            return items;
        }

        public IList<SelectListItem> GetCoachList(IList<Coach> coaches)
        {
            var items = new List<SelectListItem>();
            if (coaches == null)
                return items;
            foreach (var item in coaches.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList())
            {
                items.Add(new SelectListItem { Text = string.Format("{0}, {1}", item.LastName, item.FirstName), Value = item.Id.ToString() });
            }
            return items;
        }

        public IList<SelectListItem> GetPlayerList(IList<Player> players)
        {
            var items = new List<SelectListItem>();
            if (players == null)
                return items;
            foreach (var item in players.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList())
            {
                items.Add(new SelectListItem { Text = string.Format("{0}, {1}", item.LastName, item.FirstName), Value = item.Id.ToString() });
            }
            return items;
        }

        public IList<SelectListItem> GetGuardianList(IList<Guardian> guardians)
        {
            var items = new List<SelectListItem>();
            if (guardians == null)
                return items;
            foreach (var item in guardians.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList())
            {
                items.Add(new SelectListItem { Text = string.Format("{0}, {1}", item.LastName, item.FirstName), Value = item.Id.ToString() });
            }
            return items;
        }

        public static IList<string> GetEmailRecipientsForLeague(League league)
        {
            var emails = new List<string>();
            if (league == null)
                return emails;
            foreach (var team in league.Teams)
            {
                emails.AddRange(GetEmailRecipientsForTeam(team));
            }
            return emails.Distinct().ToList();
        }

        public static IList<string> GetEmailRecipientsForPlayer(Player player)
        {
            var emails = new List<string>();
            if (player == null)
                return emails;
            if (!string.IsNullOrEmpty(player.Email))
                emails.Add(player.Email);
            if (player.User != null)
            {
                if (!string.IsNullOrEmpty(player.User.Email))
                    emails.Add(player.User.Email);
            }
            return emails.Distinct().ToList();
        }

        public static IList<string> GetEmailRecipientsForGuardian(Guardian guardian)
        {
            var emails = new List<string>();
            if (guardian == null)
                return emails;
            if (!string.IsNullOrEmpty(guardian.Email))
                emails.Add(guardian.Email);

            if (guardian.User != null)
            {
                if (!string.IsNullOrEmpty(guardian.User.Email))
                    emails.Add(guardian.User.Email);
            }
            return emails.Distinct().ToList();
        }

        public static IList<string> GetEmailRecipientsForCoach(Coach coach)
        {
            var emails = new List<string>();
            if (coach == null)
                return emails;
            if (!string.IsNullOrEmpty(coach.Email))
                emails.Add(coach.Email);

            if (coach.User != null)
            {
                if (!string.IsNullOrEmpty(coach.User.Email))
                    emails.Add(coach.User.Email);
            }
            return emails.Distinct().ToList();
        }

        public static IList<string> GetEmailRecipientsForManager(Manager manager)
        {
            var emails = new List<string>();
            if (manager == null)
                return emails;
            if (!string.IsNullOrEmpty(manager.Email))
                emails.Add(manager.Email);

            if (manager.User != null)
            {
                if (!string.IsNullOrEmpty(manager.User.Email))
                    emails.Add(manager.User.Email);
            }
            return emails.Distinct().ToList();
        }

        public static IList<string> GetEmailRecipientsForTeam(Team team)
        {
            var emails = new List<string>();
            if (team == null)
                return emails;
            foreach (var player in team.Players)
            {
                emails.AddRange(GetEmailRecipientsForPlayer(player.Player));

                if (player.Player.Guardian != null)
                {
                    emails.AddRange(GetEmailRecipientsForGuardian(player.Player.Guardian));
                }
            }
            foreach (var coach in team.Coaches)
            {
                emails.AddRange(GetEmailRecipientsForCoach(coach));
            }
            foreach (var manager in team.Managers)
            {
                emails.AddRange(GetEmailRecipientsForManager(manager));
            }
            return emails.Distinct().ToList();
        }


        public ActionResult Index()
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var model = new MessageCreateModel();
            model.SentMessages = MessageLog.GetMessageLogsForUser(user);

            IList<League> leagues = new List<League>();
            if (user.Role == UserRole.Administrator)
            {
                leagues = League.GetAllLeagues();
            }
            
            var teams = Team.GetTeamsForUser(user);
            var players = Player.GetPlayersUserCanMessage(user);
            var playersFreeAgent = Player.GetFreeAgentPlayersUserCanMessage(user);
            var coaches = Coach.GetCoachesUserCanMessage(user);
            var guardians = Guardian.GetGuardiansUserCanMessage(user);
            var guardiansFreeAgent = Guardian.GetFreeAgentGuardiansUserCanMessage(user);

            ViewBag.TeamList = GetTeamList(teams);
            ViewBag.PlayerList = GetPlayerList(players);
            ViewBag.FreeAgentPlayerList = GetPlayerList(playersFreeAgent);
            ViewBag.CoachList = GetCoachList(coaches);
            ViewBag.LeagueList = GetLeagueList(leagues);
            ViewBag.GuardianList = GetGuardianList(guardians);
            ViewBag.FreeAgentGuardianList = GetGuardianList(guardiansFreeAgent);

            model.CanMessageLeagues = (user.Role == UserRole.Administrator);
            model.CanMessageUmpires = model.CanMessageLeagues;

            model.Leagues = new List<League>();
            model.Teams = new List<Team>();
            model.Players = new List<Player>();
            model.Coaches = new List<Coach>();
            model.Guardians = new List<Guardian>();
            model.LeagueIds = new List<int>();
            model.TeamIds = new List<int>();
            model.CoachIds = new List<int>();
            model.PlayerIds = new List<int>();
            model.GuardianIds = new List<int>();
            if (user.Role == UserRole.Administrator)
            {
                foreach (var item in model.LeagueIds)
                {
                    model.Leagues.Add(leagues.Distinct().Where(i => i.Id == item).Single());
                }                
            }

            foreach (var item in model.TeamIds)
            {
                model.Teams.Add(teams.Distinct().Where(i => i.Id == item).Single());
            }

            foreach (var item in model.CoachIds)
            {
                model.Coaches.Add(coaches.Distinct().Where(i => i.Id == item).Single());
            }

            foreach (var item in model.PlayerIds)
            {
                model.Players.Add(players.Distinct().Where(i => i.Id == item).Single());
            }
            foreach (var item in model.GuardianIds)
            {
                model.Guardians.Add(guardians.Distinct().Where(i => i.Id == item).Single());
            }

            return View(model);
        }

        public static string MessageFooterForPlayer(Player player)
        {
            return player.LastName + ", " + player.FirstName + (player.DateOfBirth.HasValue ? " " + PlayerHelper.PlayerAge(player.DateOfBirth.Value).ToString() + "U" : "") + " " + player.Email + "\n";
        }
        public static string MessageFooterBasedOnUser(User user)
        {
            if (user.Role == UserRole.Administrator)
                return "\n\nSent to you from the Administrator " + user.Email;
            if (user.Role == UserRole.Coach)
                return "\n\nSent to you from the Coach " + user.Email;
            if (user.Role == UserRole.Player)
                return "\n\nSent to you from the Player " + MessageFooterForPlayer(Player.GetPlayerForUser(user));
            if (user.Role == UserRole.Guardian)
            {
                var msg = "\n\nSent to you from the Guardian " + user.Email + " with players:\n";
                var guardian = Guardian.GetGuardianForUser(user);
                foreach (var player in guardian.Players)
                {
                    msg += MessageFooterForPlayer(player);
                }
                return msg;
            }
            return "";
        }

        [HttpPost]
        public JsonResult SendMessage(string subject, string body, int? playerId, int? leagueId, int? teamId, int? coachId, int? guardianId)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            var recipients = new List<string>();
            IList<League> leagues = new List<League>();
            if (user.Role == UserRole.Administrator)
            {
                leagues = League.GetAllLeagues();
            }
            var teams = Team.GetTeamsForUser(user);
            var players = Player.GetPlayersUserCanMessage(user);
            var coaches = Coach.GetCoachesUserCanMessage(user);
            var guardians = Guardian.GetGuardiansUserCanMessage(user);

            if (playerId.HasValue)
            {
                var player = players.Distinct().Where(p => p.Id == playerId.Value).SingleOrDefault();
                if (player != null)
                {
                    recipients.AddRange(GetEmailRecipientsForPlayer(player));
                }
            }
             else if (leagueId.HasValue)
            {
                var league = leagues.Distinct().Where(p => p.Id == leagueId.Value).SingleOrDefault();
                if (league != null)
                {
                    recipients.AddRange(GetEmailRecipientsForLeague(league));
                }
            }
            else if (teamId.HasValue)
            {
                var team = teams.Distinct().Where(p => p.Id == teamId.Value).SingleOrDefault();
                if (team != null)
                {
                    recipients.AddRange(GetEmailRecipientsForTeam(team));
                }
            }
            else if (coachId.HasValue)
            {
                var coach = Coach.GetCoachById(coachId.Value);// coaches.Distinct().Where(p => p.Id == coachId.Value).SingleOrDefault();
                if (coach != null)
                {
                    recipients.AddRange(GetEmailRecipientsForCoach(coach));
                }
            }
            else if (guardianId.HasValue)
            {
                var guardian = guardians.Distinct().Where(p => p.Id == guardianId.Value).SingleOrDefault();
                if (guardian != null)
                {
                    recipients.AddRange(GetEmailRecipientsForGuardian(guardian));
                }
            }

            var ret = new AjaxResponse();
            ret.Success = false;

            if (recipients.Contains(user.Email))
                recipients.Remove(user.Email);

            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                ret.Error = "You must enter a subject and message.";
                return this.Json(ret);
            }


            if (recipients.Count == 0)
            {
                ret.Error = "There are no email recipients for the item you have selected.";
                return this.Json(ret);
            }

            try
            {
                SendMessageToRecipients(body + MessageFooterBasedOnUser(user), subject, recipients, user);
                ret.Success = true;
            }
            catch (Exception e)
            {
                ret.Error = "An error occurred while trying to send the message.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return this.Json(ret);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MessageCreateModel model)
        {
            var user = Web.Models.User.GetUserByEmail(User.Identity.Name);
            model.SentMessages = MessageLog.GetMessageLogsForUser(user);
            IList<League> leagues = new List<League>();
            if (user.Role == UserRole.Administrator)
            {
                leagues = League.GetAllLeagues();
            }
            var teams = Team.GetTeamsForUser(user);
            var players = Player.GetPlayersUserCanMessage(user);
            var coaches = Coach.GetCoachesUserCanMessage(user);
            var guardians = Guardian.GetGuardiansUserCanMessage(user);

            ViewBag.TeamList = GetTeamList(teams);
            ViewBag.PlayerList = GetPlayerList(players);
            ViewBag.CoachList = GetCoachList(coaches);
            ViewBag.LeagueList = GetLeagueList(leagues);
            ViewBag.GuardianList = GetGuardianList(guardians);

            model.CanMessageLeagues = (user.Role == UserRole.Administrator);
            model.CanMessageUmpires = model.CanMessageLeagues;

            model.Leagues = new List<League>();
            model.Teams = new List<Team>();
            model.Players = new List<Player>();
            model.Coaches = new List<Coach>();
            model.Guardians = new List<Guardian>();
            
            if (user.Role == UserRole.Administrator)
            {
                if (model.LeagueIds != null)
                {
                    foreach (var item in model.LeagueIds)
                    {
                        model.Leagues.Add(leagues.Distinct().Where(i => i.Id == item).Single());
                    }
                }
            }
            if (model.TeamIds != null)
            {
                foreach (var item in model.TeamIds)
                {

                    model.Teams.Add(teams.Distinct().Where(i => i.Id == item).Single());
                }
            }

            if (model.CoachIds != null)
            {
                foreach (var item in model.CoachIds)
                {
                    model.Coaches.Add(coaches.Distinct().Where(i => i.Id == item).Single());
                }
            }

            if (model.PlayerIds != null)
            {
                foreach (var item in model.PlayerIds)
                {
                    model.Players.Add(players.Distinct().Where(i => i.Id == item).Single());
                }
            }

            if (model.GuardianIds != null)
            {
                foreach (var item in model.GuardianIds)
                {
                    model.Guardians.Add(guardians.Distinct().Where(i => i.Id == item).Single());
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var recipients = new List<string>();
            switch (model.MessageTo)
            {
                case "League":
                    foreach (var league in model.Leagues)
                    {
                        // all teams for league
                        recipients.AddRange(GetEmailRecipientsForLeague(league));
                    }
                    break;
                case "Player":
                    foreach (var player in model.Players)
                    {
                        recipients.AddRange(GetEmailRecipientsForPlayer(player));
                    }
                    break;
                case "Coach":
                    foreach (var coach in model.Coaches)
                    {
                        recipients.AddRange(GetEmailRecipientsForCoach(coach));
                    }
                    break;
                case "Guardian":
                    foreach (var item in model.Guardians)
                    {
                        recipients.AddRange(GetEmailRecipientsForGuardian(item));
                    }
                    break;
                case "Team":
                    foreach (var item in model.Teams)
                    {
                        recipients.AddRange(GetEmailRecipientsForTeam(item));
                    }
                    break;
                case "Site":
                    recipients.Add(ConfigurationManager.AppSettings["SiteEmail"]);
                    break;
            }

            recipients = recipients.Distinct().ToList();
            if (recipients.Contains(user.Email))
                recipients.Remove(user.Email);

            if (recipients.Count == 0)
            {
                ModelState.AddModelError("", "There are no recipients for the items you have selected.");
                return View(model);
            }
            
            try
            {
                SendMessageToRecipients(model.Body, model.Subject, recipients, user);
                TempData["MessageSent"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while trying to send the message.";
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return View(model);
            }
        }

        public static void SendMessageToRecipients(string body, string subject, List<string> recipients, User sender = null)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var message = new MailMessage())
            {
                if (sender == null)
                {
                    message.From = new MailAddress("noreply@bltbaseball.com");
                }
                else
                {
                    message.To.Add(new MailAddress(sender.Email));
                    message.From = new MailAddress(sender.Email);
                }
                foreach (var email in recipients)
                {
                    try
                    {
                        if (sender == null)
                            message.To.Add(new MailAddress("noreply@bltbaseball.com"));
                        else
                            message.Bcc.Add(new MailAddress(email));
                    }
                    catch (Exception)
                    {

                    }
                }
                message.Subject = String.Format("[iScrimmage] {0}", subject);
                message.Body = body;
                message.IsBodyHtml = false;
                using (var client = new SmtpClient())
                {
                    client.Send(message);
                }
            }

            using (var tx = session.BeginTransaction())
            {
                var log = new MessageLog();
                log.SentBy = sender;
                log.SentOn = DateTime.Now;
                log.Subject = subject.Substring(0, Math.Min(4000, subject.Length));
                log.Body = body.Substring(0, Math.Min(4000, body.Length));
                log.RecipientCount = recipients.Count;
                log.Recipients = string.Join(",", recipients);
                log.Recipients = log.Recipients.Substring(0, Math.Min(4000, log.Recipients.Length));
                session.Save(log);
                tx.Commit();
            }
        }
    }
}
