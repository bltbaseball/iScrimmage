using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using Web.Models;

namespace Web.Helpers
{
    public class EmailNotification
    {
        public const string TEMPLATE_PATH = "~/App_Data/EmailTemplates/";

        public static void GamePlayNotification(Game game)
        {
            if (game.HomeTeam != null)
                GamePlayNotification(game, game.HomeTeam);
            if (game.AwayTeam != null)
                GamePlayNotification(game, game.AwayTeam);
        }

        public static void GamePlayNotification(Game game, Team team)
        {
            var games = new List<Game>();
            games.Add(game);
            var leagueGames = LeagueOverviewModel.MapGamesToModel(games);
            var gameData = ToTableData<LeagueGameModel>(leagueGames.ToList(), new string[] { "HomeTeamName", "AwayTeamName", "GameTime", "Location", "Field", "Type" });
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-gameNotification.html"));
            body = body.Replace("{TeamName}", Team.PrettyNameWithoutLeague(team));
            body = body.Replace("{GameDate}", game.GameDate.ToShortDateString() + " " + game.GameDate.ToShortTimeString());
            body = body.Replace("{GameGrid}", gameData);
            body = body.Replace("{LeagueName}", team.League.Name);
            body = body.Replace("{OverviewURL}", string.Format("{0}Overview/Details/{1}", ConfigurationManager.AppSettings["SiteUrl"], team.League.Id));

            var subject = "iScrimmage Game " + game.GameDate.ToShortDateString() + " " + game.GameDate.ToShortTimeString();
            SendEmailToCoachesAndManagers(subject, body, team);
            SendEmailToPlayersAndGuardians(subject, body, team);
            SendEmail(subject, body, "adrian@bltbaseball.com");
        }

        public static void GameUpdateNotification(Game game)
        {
            if (game.HomeTeam != null)
                GameUpdateNotification(game, game.HomeTeam);
            if (game.AwayTeam != null)
                GameUpdateNotification(game, game.AwayTeam);
        }
        
        public static void GameUpdateNotification(Game game, Team team)
        {
            var games = new List<Game>();
            games.Add(game);
            var leagueGames = LeagueOverviewModel.MapGamesToModel(games);
            var gameData = ToTableData<LeagueGameModel>(leagueGames.ToList(), new string[] { "HomeTeamName", "AwayTeamName", "GameTime", "Location", "Field", "Type" });
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-updated.html"));
            body = body.Replace("{TeamName}", Team.PrettyNameWithoutLeague(team));
            body = body.Replace("{GameDate}", game.GameDate.ToShortDateString() + " " + game.GameDate.ToShortTimeString());
            body = body.Replace("{GameGrid}", gameData);
            body = body.Replace("{LeagueName}", team.League.Name);
            body = body.Replace("{OverviewURL}", string.Format("{0}Overview/Details/{1}", ConfigurationManager.AppSettings["SiteUrl"], team.League.Id));

            var subject = "iScrimmage Game Update " + game.GameDate.ToShortDateString() + " " + game.GameDate.ToShortTimeString();
            SendEmailToCoachesAndManagers(subject, body, team);
            SendEmailToPlayersAndGuardians(subject, body, team);
            SendEmail(subject, body, "adrian@bltbaseball.com");
        }

        public static void GamesRequested(Game game, User requestor, Team teamToPlay, Team teamToNotify)
        {
            var games = new List<Game>();
            games.Add(game);
            var gameData = ToTableData<Game>(games.ToList(), new string[] { "HomeTeam", "AwayTeam", "GameDate", "Location", "Status" });
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-requested.html"));
            switch (requestor.Role)
            {
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(requestor);
                    body = body.Replace("{InvitingUser}", string.Format("{0} {1}", coach.FirstName, coach.LastName));
                    break;
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(requestor);
                    body = body.Replace("{InvitingUser}", string.Format("{0} {1}", manager.FirstName, manager.LastName));
                    break;
                case UserRole.Administrator:
                    body = body.Replace("{InvitingUser}", string.Format("{0} {1}", requestor.FirstName, requestor.LastName));
                    break;
            }
            body = body.Replace("{TeamName}", Team.PrettyName(teamToPlay));
            body = body.Replace("{GameDate}", game.GameDate.ToShortDateString() + " " + game.GameDate.ToShortTimeString());
            body = body.Replace("{GameGrid}", gameData);

            var subject = "iScrimmage Game Request";
            SendEmailToCoachesAndManagers(subject, body, teamToNotify);
            //SendEmail(subject, body, "jeff@triggerinc.com");
        }

        public static void GamesConfirmed(Game game, Team teamToNotify)
        {
            var games = new List<Game>();
            games.Add(game);
            var gameData = ToTableData<Game>(games.ToList(), new string[] { "HomeTeam", "AwayTeam", "GameDate", "Location", "Status" });
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-scheduleConfirmedCoach.html"));
            body = body.Replace("{TeamName}", Team.PrettyName(teamToNotify));
            body = body.Replace("{GameGrid}", gameData);

            var subject = "iScrimmage Game Accepted";
            SendEmailToCoachesAndManagers(subject, body, teamToNotify);
            //SendEmail(subject, body, "jeff@triggerinc.com");

            body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-scheduleConfirmedPlayer.html"));
            body = body.Replace("{TeamName}", Team.PrettyName(teamToNotify));
            body = body.Replace("{GameGrid}", gameData);

            SendEmailToPlayersAndGuardians(subject, body, teamToNotify);
        }

        public static void GamesNewAvailableDates(IList<AvailableDates> dates, Coach coach, Team team)
        {
            var gameData = ToTableData<AvailableDates>(dates.ToList(), new string[] { "Team", "Date", "Location", "IsHome", "IsAway", "DistanceFromLocation" });
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "games-newAvailableDates.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", coach.FirstName, coach.LastName));
            body = body.Replace("{TeamName}", Team.PrettyName(team));
            body = body.Replace("{GameGrid}", gameData);

            var subject = "iScrimmage New Games Available";
            SendEmail(subject, body, coach.Email);
        }

        public static string UserFirstName(User user)
        {
            if (user.Role == UserRole.Coach)
                return Coach.GetCoachForUser(user).FirstName;
            if (user.Role == UserRole.Manager)
                return Manager.GetManagerForUser(user).FirstName;
            if (user.Role == UserRole.Guardian)
                return Guardian.GetGuardianForUser(user).FirstName;
            if (user.Role == UserRole.Player)
                return Player.GetPlayerForUser(user).FirstName;
            if (user.Role == UserRole.Administrator)
                return user.FirstName;
            return "";
        }

        public static string UserLastName(User user)
        {
            if (user.Role == UserRole.Coach)
                return Coach.GetCoachForUser(user).LastName;
            if (user.Role == UserRole.Manager)
                return Manager.GetManagerForUser(user).LastName;
            if (user.Role == UserRole.Guardian)
                return Guardian.GetGuardianForUser(user).LastName;
            if (user.Role == UserRole.Player)
                return Player.GetPlayerForUser(user).LastName;
            if (user.Role == UserRole.Administrator)
                return user.LastName;
            return "";
        }

        public static void InviteCoach(Coach coach, User inviter)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "invite-coach.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", coach.FirstName, coach.LastName));
            body = body.Replace("{Inviter}", string.Format("{0} {1}", UserFirstName(inviter), UserLastName(inviter)));
            body = body.Replace("{RegisterURL}", string.Format("{0}Invitation/Coach/{1}/{2}", ConfigurationManager.AppSettings["SiteUrl"], coach.Id, coach.InviteToken));
            var subject = "Invitation To Join iScrimmage As A Coach";
            SendEmail(subject, body, coach.Email);
        }

        public static void InviteCoachChallenge(Coach coach, User inviter)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "challenge-coach.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", coach.FirstName, coach.LastName));
            body = body.Replace("{Inviter}", string.Format("{0} {1}", UserFirstName(inviter), UserLastName(inviter)));
            body = body.Replace("{RegisterURL}", string.Format("{0}Invitation/Coach/{1}/{2}", ConfigurationManager.AppSettings["SiteUrl"], coach.Id, coach.InviteToken));
            var subject = "You Have Been Challenged on iScrimmage";
            SendEmail(subject, body, coach.Email);
        }

        public static void InviteGuardian(Guardian guardian, User inviter)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "invite-guardian.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", guardian.FirstName, guardian.LastName));
            body = body.Replace("{Inviter}", string.Format("{0} {1}", UserFirstName(inviter), UserLastName(inviter)));
            body = body.Replace("{RegisterURL}", string.Format("{0}Invitation/Guardian/{1}/{2}", ConfigurationManager.AppSettings["SiteUrl"], guardian.Id, guardian.InviteToken));
            var subject = "Invitation To Join iScrimmage As A Guardian";
            SendEmail(subject, body, guardian.Email);
        }

        public static void InviteManager(Manager manager, User inviter)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "invite-manager.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", manager.FirstName, manager.LastName));
            body = body.Replace("{Inviter}", string.Format("{0} {1}", UserFirstName(inviter), UserLastName(inviter)));
            body = body.Replace("{RegisterURL}", string.Format("{0}Invitation/Manager/{1}/{2}", ConfigurationManager.AppSettings["SiteUrl"], manager.Id, manager.InviteToken));
            var subject = "Invitation To Join iScrimmage As A Manager";
            SendEmail(subject, body, manager.Email);
        }

        public static void InvitePlayer(Player player, User inviter)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "invite-player.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", player.FirstName, player.LastName));
            body = body.Replace("{Inviter}", string.Format("{0} {1}", UserFirstName(inviter), UserLastName(inviter)));
            body = body.Replace("{RegisterURL}", string.Format("{0}Invitation/Player/{1}/{2}", ConfigurationManager.AppSettings["SiteUrl"], player.Id, player.InviteToken));
            var subject = "Invitation To Join iScrimmage As A Player";
            SendEmail(subject, body, player.Email);
        }

        /* Not used as it is the same as new-player-team
        public static void PlayerPickup(Player player, Team team)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "player-pickup.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", player.FirstName, player.LastName));
            body = body.Replace("{TeamName}", team.Name);

            var subject = "Added To Team";
            SendEmail(subject, body, player.Email);
        }*/

        public static void NewTeam(Team team, Manager manager)
        {
            NewTeam(team, string.Format("{0} {1}", manager.FirstName, manager.LastName), manager.Email);
        }

        public static void NewTeam(Team team, Coach coach)
        {
            NewTeam(team, string.Format("{0} {1}", coach.FirstName, coach.LastName), coach.Email);
        }

        public static void NewTeam(Team team, string name, string email)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "new-team.html"));
            body = body.Replace("{Name}", name);
            body = body.Replace("{TeamName}", team.Name);

            var subject = "Team Created";
            
            SendEmail(subject, body, email);
        }

        public static void NewCoach(Coach coach)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "welcome-coach.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", coach.FirstName, coach.LastName));

            var subject = "Welcome Coach";
            SendEmail(subject, body, coach.Email);
        }

        public static void NewManager(Manager manager)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "welcome-manager.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", manager.FirstName, manager.LastName));

            var subject = "Welcome Manager";
            SendEmail(subject, body, manager.Email);
        }

        public static void NewGuardian(Guardian guardian)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "welcome-guardian.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", guardian.FirstName, guardian.LastName));

            var subject = "Welcome Guardian";
            SendEmail(subject, body, guardian.Email);
        }

        public static void NewPlayer(Player player)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "welcome-player.html"));
            body = body.Replace("{Name}", string.Format("{0} {1}", player.FirstName, player.LastName));

            var subject = "Welcome Player";
            SendEmail(subject, body, player.Email);
        }

        public static void NewFreeAgentPlayer(Player player)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "new-player-freeagent.html"));
            //body = body.Replace("{Name}", name);
            body = body.Replace("{PlayerName}", string.Format("{0} {1}", player.FirstName, player.LastName));

            var subject = "Player Created";
            SendEmail(subject, body, player.Email);
        }

        public static void PlayerAddedToTeam(TeamPlayer player)
        {
            var body = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "new-player-team.html"));
            //body = body.Replace("{Name}", name);
            body = body.Replace("{PlayerName}", string.Format("{0} {1}", player.Player.FirstName, player.Player.LastName));
            body = body.Replace("{TeamName}", player.Team.Name);

            var subject = "Player Added To Team";
            SendEmail(subject, body, player.Player.Email);
        }

        private static void SendEmailToCoachesAndManagers(string subject, string body, Team team)
        {
            foreach (var coach in team.Coaches)
            {
                SendEmail(subject, body.Replace("{Name}", string.Format("{0} {1}", coach.FirstName, coach.LastName)), coach.Email);
            }
            foreach (var manager in team.Managers)
            {
                SendEmail(subject, body.Replace("{Name}", string.Format("{0} {1}", manager.FirstName, manager.LastName)), manager.Email);
            }
        }

        private static void SendEmailToPlayersAndGuardians(string subject, string body, Team team)
        {
            foreach (var player in team.Players)
            {
                SendEmail(subject, body.Replace("{Name}", string.Format("{0} {1}", player.Player.FirstName, player.Player.LastName)), player.Player.Email);
                if(player.Player.Guardian != null)
                    SendEmail(subject, body.Replace("{Name}", string.Format("{0} {1}", player.Player.Guardian.FirstName, player.Player.Guardian.LastName)), player.Player.Guardian.Email);
            }
        }

        private static void SendEmail(string subject, string body, string recipient)
        {
            if (string.IsNullOrEmpty(recipient))
                return;

            try
            {
                var template = File.ReadAllText(HttpContext.Current.Server.MapPath(TEMPLATE_PATH + "layout.html"));
                var email = template.Replace("{Body}", body);
                var msg = new MailMessage();
                msg.To.Add(new MailAddress(recipient));
                msg.From = new MailAddress("noreply@iscrimmage.com");
                msg.Subject = subject;
                msg.Body = email;
                msg.IsBodyHtml = true;
                using (var client = new SmtpClient())
                {
                    client.Send(msg);
                }
            }
            catch (Exception e)
            {
            }
        }

        public static string ToTableData<type>(List<type> obj, string[] columns)
        {
            StringBuilder ret = new StringBuilder();
            Boolean alt = false;

            foreach (var row in obj)
            {
                ret.Append("<tr style='text-align:center;" + (alt ? " background-color: #e5e5e5;" : "") + "'>");
                foreach (var column in columns)
                {
                    foreach (var field in row.GetType().GetProperties()) // Loop through fields
                    {
                        if (field.Name != column)
                            continue;
                        object temp = field.GetValue(row, null); // Get value
                        if (temp is int) // See if it is an integer.
                        {
                            int value = (int)temp;
                            ret.Append("<td>" + value.ToString() + " </td>");
                        }
                        else if (temp is Nullable<int>) // See if it is an integer.
                        {
                            int? value = (int?)temp;
                            if (value.HasValue)
                                ret.Append("<td>" + value.ToString() + " </td>");
                            else
                                ret.Append("<td>&nbsp; </td>");
                        }
                        else if (temp is string) // See if it is a string.
                        {
                            string value = temp as string;
                            ret.Append("<td>" + value + "</td>");
                        }
                        else if (temp is GameStatus) // See if it is a string.
                        {
                            GameStatus value = (GameStatus)temp;
                            ret.Append("<td>" + value.ToString() + "</td>");
                        }
                        else if (temp is bool) // See if it is an integer.
                        {
                            bool value = (bool)temp;
                            if (value)
                                ret.Append("<td>X</td>");
                            else
                                ret.Append("<td>&nbsp; </td>");
                        }
                        else if (temp is DateTime) // See if it is a date.
                        {
                            DateTime value = (DateTime)temp;
                            ret.Append("<td>" + value.ToShortDateString() + " " + value.ToShortTimeString() + "</td>");
                        }
                        else if (temp is Team) // See if it is a team.
                        {
                            Team value = (Team)temp;
                            ret.Append("<td><a href='" + ConfigurationManager.AppSettings["SiteUrl"] + "/Overview/Team/" + value.Id + "'>" + Team.PrettyName(value) + "</a></td>");
                        }
                        else if (temp is Location) // See if it is a location.
                        {
                            Location value = (Location)temp;
                            ret.Append("<td><a href='" + ConfigurationManager.AppSettings["SiteUrl"] + "/Overview/Location/" + value.Id + "'>" + value.Name + "</a></td>");
                        }
                        else
                        {
                            ret.Append("<td>&nbsp; </td>");
                        }
                    }
                }
                ret.Append("</tr>");
                alt = !alt;
            }
            return ret.ToString();
        }
    }
}