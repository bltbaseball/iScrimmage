using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Coach
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Photo { get; set; }
        public virtual string PhotoType { get; set; }
        public virtual User User { get; set; }
        public virtual string InviteToken { get; set; }
        public virtual DateTime? InvitationSentOn { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual IList<Team> Teams { get; set; }

        public static IList<Coach> GetAllCoaches()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Coach>().List();
        }

        /// <summary>
        /// A Manager has access to coaches on his assigned teams. An Administrator has access to all coaches.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IList<Coach> GetCoachesForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            if (user.Role == UserRole.Administrator)
            {
                return GetAllCoaches();
            }
            else if (user.Role == UserRole.Manager)
            {
                var manager = Manager.GetManagerForUser(user);
                if (manager == null)
                    throw new ApplicationException(string.Format("Manager not found for User #{0}", user.Id));
                var teams = Team.GetTeamsWithManager(manager);
                var coaches = new List<Coach>();
                foreach (var team in teams)
                {
                    coaches.AddRange(team.Coaches);
                }
                return coaches.Distinct().ToList();
            }
            else // if (user.Role == UserRole.Coach)
            {
                // get coaches on the same team as this coach
                var coach = Coach.GetCoachForUser(user);
                if (coach == null)
                    throw new ApplicationException(string.Format("Coach not found for User #{0}", user.Id));
                var teams = Team.GetTeamsWithCoach(coach);
                var coaches = new List<Coach>();
                foreach (var team in teams)
                {
                    coaches.AddRange(team.Coaches);
                }
                return coaches.Distinct().ToList();
            }
        }

        public static IList<Coach> GetCoachesUserCanMessage(User user)
        {
            switch (user.Role)
            {
                case UserRole.Administrator:
                case UserRole.Coach:
                case UserRole.Manager:
                    return Coach.GetAllCoaches();//.GetCoachesForUser(user);
                case UserRole.Player:
                    return Coach.GetAllCoaches();//.GetCoachesForUser(user);
                    /*// get coaches on this player's teams
                    var player = Player.GetPlayerForUser(user);
                    var playerCoaches = new List<Coach>();
                    foreach (var team in player.Teams)
                    {
                        playerCoaches.AddRange(team.Team.Coaches);
                    }
                    return playerCoaches.Distinct().ToList();*/
                case UserRole.Guardian:
                    return Coach.GetAllCoaches();//.GetCoachesForUser(user);
                    /*var players = Player.GetPlayersForUser(user);
                    var guardianCoaches = new List<Coach>();
                    foreach (var guardianPlayer in players)
                    {
                        foreach (var team in guardianPlayer.Teams)
                        {
                            guardianCoaches.AddRange(team.Team.Coaches);
                        }
                    }
                    return guardianCoaches.Distinct().ToList();*/
            }
            return null;
        }

        public static Coach GetCoachForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Coach>().Where(c => c.User == user).List().SingleOrDefault();
        }

        public static Coach GetCoachForEmail(String email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Coach>().Where(c => c.Email == email).List().FirstOrDefault();
        }

        public static Coach GetCoachById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Coach>().Where(c => c.Id == id).List().FirstOrDefault();
        }

        public static Coach GetCoachById(int id, User user)
        {
            var coaches = GetCoachesForUser(user);
            return coaches.Where(c => c.Id == id).SingleOrDefault();
        }

        public static Coach GetCoachWithInviteToken(string token)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Coach>().Where(c => c.InviteToken == token).List().SingleOrDefault();
        }
    }
}