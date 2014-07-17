using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// Parental guardian for a Player, as all Players are underage. (Used Guardian instead of Parent because Parent could be a reserved name in certain scenarios.)
    /// </summary>
    public class Guardian
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string InviteToken { get; set; }
        public virtual DateTime? InvitationSentOn { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual User User { get; set; }
        public virtual IList<Player> Players { get; set; }

        public static IList<Guardian> GetAllGuardians()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().List();
        }

        public static Guardian GetGuardianForUser(Models.User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.User == user).SingleOrDefault();
        }

        public static Guardian GetGuardianForEmail(String email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.Email == email).List().FirstOrDefault();
        }

        public static Guardian GetGuardianWithInviteToken(string token)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.InviteToken == token).List().SingleOrDefault();
        }

        public static Guardian GetGuardianByEmail(string email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.Email == email).List().SingleOrDefault();
        }

        public static Guardian GetUnlinkedGuardianByEmail(string email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.Email == email && c.User == null).SingleOrDefault();
        }

        public static Guardian GetGuardianById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Guardian>().Where(c => c.Id == id).SingleOrDefault();
        }

        public static IList<Guardian> GetGuardiansUserCanMessage(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return GetAllGuardians();
                case UserRole.Manager:
                case UserRole.Coach:
                    // coaches and managers can message guardians on their teams as well as "free agents" guardians
                    var players = new List<Player>();
                    if (user.Role == UserRole.Manager)
                    {
                        var manager = Manager.GetManagerForUser(user);
                        var teams = Team.GetTeamsWithManager(manager);
                        // these three queries could probably be combined
                        var managerGuardians = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .JoinQueryOver<Player>(c => c.Player)
                            .JoinQueryOver<Guardian>(c => c.Guardian).Select(tp => tp.Player).List<Player>().Select(tp => tp.Guardian).Distinct().ToList();
//                        managerGuardians.AddRange(Player.GetFreeAgentPlayers().Where(p => p.Guardian != null).Select(p => p.Guardian).Distinct().ToList());
                        return managerGuardians;
                    }
                    else
                    {
                        var coach = Coach.GetCoachForUser(user);
                        var teams = Team.GetTeamsWithCoach(coach);
                        // these three queries could probably be combined
                        var coachGuardians = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .JoinQueryOver<Player>(c => c.Player)
                            .JoinQueryOver<Guardian>(c => c.Guardian).Select(tp => tp.Player).List<Player>().Select(tp => tp.Guardian).Distinct().ToList();
//                        coachGuardians.AddRange(Player.GetFreeAgentPlayers().Where(p => p.Guardian != null).Select(p => p.Guardian).Distinct().ToList());
                        return coachGuardians;
                    }

                case UserRole.Guardian:
                    var guardian = Guardian.GetGuardianForUser(user);
                    var guardianPlayers = Player.GetPlayersWithGuardian(guardian);
                    // guardians can message guardians for players on their players' teams
                    var allTeams = new List<Team>();
                    foreach (var gplayer in guardianPlayers)
                    {
                        allTeams.AddRange(gplayer.Teams.Select(t => t.Team).Distinct().ToList());
                    }
                    var guardianGuardians = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(allTeams.ToArray())
                            .JoinQueryOver<Player>(c => c.Player)
                            .JoinQueryOver<Guardian>(c => c.Guardian).Select(tp => tp.Player).List<Player>().Select(tp => tp.Guardian).Distinct().ToList();
                    return guardianGuardians;
                case UserRole.Player:
                    // player can message other players on their team's guardians
                    var player = Player.GetPlayerForUser(user);
                    var teamGuardians = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(player.Teams.Select(t => t.Team).Distinct().ToArray())
                            .JoinQueryOver<Player>(c => c.Player)
                            .JoinQueryOver<Guardian>(c => c.Guardian).Select(tp => tp.Player).List<Player>().Select(tp => tp.Guardian).Distinct().ToList();
                    return teamGuardians;
            }
            return null;
        }

        public static IList<Guardian> GetFreeAgentGuardiansUserCanMessage(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var players = new List<Player>();
            switch (user.Role)
            {
                case UserRole.Administrator:
                case UserRole.Manager:
                case UserRole.Coach:
                    return Player.GetFreeAgentPlayers().Where(p => p.Guardian != null).Select(p => p.Guardian).Distinct().ToList(); ;
            }
            return null;
        }
    }
}