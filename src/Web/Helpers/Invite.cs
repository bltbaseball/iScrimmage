using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Web.Models;

namespace Web.Helpers
{
    public class Invite
    {
        public static void SendManagerInvite(Manager manager, User user)
        {
            if (manager.User != null)
                throw new ApplicationException("Manager already has an associated user.");

            var sb = new StringBuilder();

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                manager.InviteToken = Guid.NewGuid().ToString();
                manager.InvitationSentOn = DateTime.Now;
                session.Update(manager);

                EmailNotification.InviteManager(manager, user);

//                var msg = new MailMessage();
//                msg.To.Add(new MailAddress(emailAddress));
//                msg.From = new MailAddress("noreply@bltbaseball.com");
//                msg.Subject = "Invitation to Join BLTBaseball.com As A Manager";
//                sb.AppendFormat(
//@"Hello {0} {1},
//You have been invited to join bltbaseball.com as a Manager.
//
//There are just a few steps to get started. 
//
//If you already have a Google or Gmail account, simply click on this link and sign in {4}Invitation/Manager/{2}/{3}
//
//If you do not already have a Google or Gmail account, click this link and create one first https://accounts.google.com/SignUp
//", manager.FirstName, manager.LastName, manager.Id, manager.InviteToken, ConfigurationManager.AppSettings["SiteUrl"]);
//                msg.Body = sb.ToString();
//                msg.IsBodyHtml = false;
//                using (var client = new SmtpClient())
//                {
//                    client.Send(msg);
//                }

                tx.Commit();
            }
        }

        public static void SendPlayerInvite(Player player, User user)
        {
            //string emailAddress = player.Email;
            //if (!string.IsNullOrEmpty(emailOverride))
            //    emailAddress = emailOverride;

            if (player.User != null)
                throw new ApplicationException("Player already has an associated user.");

            var sb = new StringBuilder();

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                player.InviteToken = Guid.NewGuid().ToString();
                player.InvitationSentOn = DateTime.Now;
                session.Update(player);

                EmailNotification.InvitePlayer(player, user);

//                var msg = new MailMessage();
//                msg.To.Add(new MailAddress(emailAddress));
//                msg.From = new MailAddress("noreply@bltbaseball.com");
//                msg.Subject = "Invitation to Join BLTBaseball.com As A Player";
//                sb.AppendFormat(
//@"Hello {0} {1},
//You have been invited to join bltbaseball.com as a Player.
//
//There are just a few steps to get started. 
//
//If you already have a Google or Gmail account, simply click on this link and sign in {4}Invitation/Player/{2}/{3}
//
//If you do not already have a Google or Gmail account, click this link and create one first https://accounts.google.com/SignUp
//", player.FirstName, player.LastName, player.Id, player.InviteToken, ConfigurationManager.AppSettings["SiteUrl"]);
//                msg.Body = sb.ToString();
//                msg.IsBodyHtml = false;
//                using (var client = new SmtpClient())
//                {
//                    client.Send(msg);
//                }

                tx.Commit();
            }
        }

        public static void SendCoachInvite(Coach coach, User user)
        {
            //string emailAddress = coach.Email;
            //if (!string.IsNullOrEmpty(emailOverride))
            //    emailAddress = emailOverride;

            if (coach.User != null)
                throw new ApplicationException("Coach already has an associated user.");

            var sb = new StringBuilder();

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                coach.InviteToken = Guid.NewGuid().ToString();
                coach.InvitationSentOn = DateTime.Now;
                session.Update(coach);

                EmailNotification.InviteCoach(coach, user);

//                var msg = new MailMessage();
//                msg.To.Add(new MailAddress(emailAddress));
//                msg.From = new MailAddress("noreply@bltbaseball.com");
//                msg.Subject = "Invitation to Join BLTBaseball.com As A Coach";
//                sb.AppendFormat(
//@"Hello {0} {1},
//You have been invited to join bltbaseball.com as a Coach.
//
//There are just a few steps to get started. 
//
//If you already have a Google or Gmail account, simply click on this link and sign in {4}Invitation/Coach/{2}/{3}
//
//If you do not already have a Google or Gmail account, click this link and create one first https://accounts.google.com/SignUp
//", coach.FirstName, coach.LastName, coach.Id, coach.InviteToken, ConfigurationManager.AppSettings["SiteUrl"]);
//                msg.Body = sb.ToString();
//                msg.IsBodyHtml = false;
//                using (var client = new SmtpClient())
//                {
//                    client.Send(msg);
//                }

                tx.Commit();
            }
        }

        public static void SendUmpireInvite(Umpire item, string emailOverride = null)
        {
            string emailAddress = item.Email;
            if (!string.IsNullOrEmpty(emailOverride))
                emailAddress = emailOverride;

            if (item.User != null)
                throw new ApplicationException("Umpire already has an associated user.");

            var sb = new StringBuilder();

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                item.InviteToken = Guid.NewGuid().ToString();
                item.InvitationSentOn = DateTime.Now;
                session.Update(item);

                var msg = new MailMessage();
                msg.To.Add(new MailAddress(emailAddress));
                msg.From = new MailAddress("noreply@bltbaseball.com");
                msg.Subject = "Invitation to Join BLTBaseball.com As An Umpire";
                sb.AppendFormat(
@"Hello {0} {1},
You have been invited to join bltbaseball.com as an Umpire.

There are just a few steps to get started. 

If you already have a Google or Gmail account, simply click on this link and sign in {4}Invitation/Umpire/{2}/{3}

If you do not already have a Google or Gmail account, click this link and create one first https://accounts.google.com/SignUp
", item.FirstName, item.LastName, item.Id, item.InviteToken, ConfigurationManager.AppSettings["SiteUrl"]);
                msg.Body = sb.ToString();
                msg.IsBodyHtml = false;
                using (var client = new SmtpClient())
                {
                    client.Send(msg);
                }

                tx.Commit();
            }
        }

        public static void SendGuardianInvite(Guardian guardian, Player player, User user)
        {
            string emailAddress = guardian.Email;
            //if (!string.IsNullOrEmpty(emailOverride))
            //    emailAddress = emailOverride;

            if (guardian.User != null)
                throw new ApplicationException("Guardian already has an associated user.");

            var sb = new StringBuilder();

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                guardian.InviteToken = Guid.NewGuid().ToString();
                guardian.InvitationSentOn = DateTime.Now;
                session.Update(guardian);

                EmailNotification.InviteGuardian(guardian, user);

//                var msg = new MailMessage();
//                msg.To.Add(new MailAddress(emailAddress));
//                msg.From = new MailAddress("noreply@bltbaseball.com");
//                msg.Subject = "Invitation to Join BLTBaseball.com As A Guardian";
//                sb.AppendFormat(
//@"Hello {0} {1},
//You have been invited to join bltbaseball.com as a Guardian", guardian.FirstName, guardian.LastName);
//                if(player != null)
//                    sb.AppendFormat(@" of {0} {1}", player.FirstName, player.LastName);
//                sb.AppendFormat(@".
//
//There are just a few steps to get started. 
//
//If you already have a Google or Gmail account, simply click on this link and sign in {2}Invitation/Guardian/{0}/{1}
//
//If you do not already have a Google or Gmail account, click this link and create one first https://accounts.google.com/SignUp
//", guardian.Id, guardian.InviteToken, ConfigurationManager.AppSettings["SiteUrl"]);
//                msg.Body = sb.ToString();
//                msg.IsBodyHtml = false;
//                using (var client = new SmtpClient())
//                {
//                    client.Send(msg);
//                }

                tx.Commit();
            }
        }
    }
}