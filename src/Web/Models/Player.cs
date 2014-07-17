using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public enum PlayerStatus
    {
        Active = 1,
        Suspended = 2,
        Inactive = 0,
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
    }

    /// <summary>
    /// Player bio.
    /// </summary>
    public class Player
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime? DateOfBirth { get; set; } //Cut off date is May 1st for the age divisions
        public virtual Gender Gender { get; set; }
        public virtual Guardian Guardian { get; set; }
        public virtual string JerseyNumber { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string InviteToken { get; set; }
        public virtual DateTime? InvitationSentOn { get; set; }
        public virtual IList<TeamPlayer> Teams { get; set; }
        public virtual IList<PlayerGameStat> GameStats { get; set; }
        public virtual bool IsLookingForTeam { get; set; }
        //public virtual Fee Fee { get; set; }

        public static Player GetPlayerForUser(Models.User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Player>().Where(c => c.User == user).List().SingleOrDefault();
        }

        public static Player GetPlayerForEmail(String email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Player>().Where(c => c.Email == email).List().FirstOrDefault();
        }

        public static IList<Player> GetPlayersWithGuardian(Guardian guardian)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Player>().Where(c => c.Guardian == guardian).List();
        }

        public static IList<Player> GetAllPlayers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Player>().List();
        }

        public static Player GetUnlinkedPlayerByEmail(string email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Player>().Where(c => c.Email == email && c.User == null).SingleOrDefault();
        }

        public static IEnumerable<Player> GetFreeAgentPlayers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            var activePlayers = NHibernate.Criterion.QueryOver.Of<TeamPlayer>()
                .Select(tp => tp.Player.Id);
            return session.QueryOver<Player>()
                .WithSubquery.WhereProperty(p=>p.Id).NotIn(activePlayers)
                .List();
        }

        public static IList<Player> GetPlayersUserCanMessage(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return GetAllPlayers();
                case UserRole.Manager:
                case UserRole.Coach:
                    // coaches and managers can message players on their teams as well as "free agents"
                    var players = new List<Player>();
                    if (user.Role == UserRole.Manager)
                    {
                        var manager = Manager.GetManagerForUser(user);
                        var teams = Team.GetTeamsWithManager(manager);
                        // these three queries could probably be combined
                        var managerPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();
//                        managerPlayers.AddRange(GetFreeAgentPlayers());
                        return managerPlayers;
                    }
                    else
                    {
                        var coach = Coach.GetCoachForUser(user);
                        var teams = Team.GetTeamsWithCoach(coach);
                        // these three queries could probably be combined
                        var coachPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();
//                        coachPlayers.AddRange(GetFreeAgentPlayers());
                        return coachPlayers;
                    }

                case UserRole.Guardian:
                    var guardian = Guardian.GetGuardianForUser(user);
                    var guardianPlayers = GetPlayersWithGuardian(guardian);
                    // guardians can message players on their players' teams
                    var allTeams = new List<Team>();
                    foreach (var gplayer in guardianPlayers)
                    {
                        allTeams.AddRange(gplayer.Teams.Select(t => t.Team).Distinct().ToList());
                    }
                    guardianPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(allTeams.ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();
                    return guardianPlayers;
                case UserRole.Player:
                    // player can message other players on their team
                    var player = Player.GetPlayerForUser(user);
                    var teamPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(player.Teams.Select(t => t.Team).Distinct().ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();
                    return teamPlayers;
            }
            return null;
        }

        public static IList<Player> GetFreeAgentPlayersUserCanMessage(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            switch (user.Role)
            {
                case UserRole.Administrator:
                case UserRole.Manager:
                case UserRole.Coach:
                    // coaches and managers can message players on their teams as well as "free agents"
                    var players = GetFreeAgentPlayers().ToList();
                    return players;
            }
            return null;
        }

        public static IList<Player> GetPlayersForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return GetAllPlayers();
                case UserRole.Manager:
                case UserRole.Coach:
                    // coaches and managers can manage players on their teams as well as "free agents"
                    var players = new List<Player>();
                    if (user.Role == UserRole.Manager)
                    {
                        var manager = Manager.GetManagerForUser(user);
                        var teams = Team.GetTeamsWithManager(manager);
                        // these three queries could probably be combined
                        var managerPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();

                        managerPlayers.AddRange(GetFreeAgentPlayers());
                        return managerPlayers;
                    }
                    else
                    {
                        var coach = Coach.GetCoachForUser(user);
                        var teams = Team.GetTeamsWithCoach(coach);
                        // these three queries could probably be combined
                        var coachPlayers = session.QueryOver<TeamPlayer>()
                            .WhereRestrictionOn(t => t.Team).IsIn(teams.ToArray())
                            .Select(tp => tp.Player).List<Player>().Distinct().ToList();
                        coachPlayers.AddRange(GetFreeAgentPlayers());
                        return coachPlayers;
                    }
                                       
                case UserRole.Guardian:
                    var guardian = Guardian.GetGuardianForUser(user);
                    return GetPlayersWithGuardian(guardian);
                default:
                    throw new ApplicationException("User not authorized to manage players.");
            }
        }

        public static Player GetPlayerById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Player>(id);
        }

        public static Player GetPlayerById(int id, User user)
        {
            return GetPlayersForUser(user).Where(p => p.Id == id).SingleOrDefault();
        }


        public static Player GetPlayerWithInviteToken(string token)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Player>().Where(c => c.InviteToken == token).List().SingleOrDefault();
        }


        public static IList<PlayerGameStat> GetPlayerGameStats(Player player)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();

            return session.QueryOver<PlayerGameStat>()
                .Where(p => p.PitchesThrown > 0 || p.InningsPitched > 0 || p.InningsOuts > 0)
                .JoinQueryOver<TeamPlayer>(c => c.TeamPlayer)
                .Where(m => m.Player.Id == player.Id)
                .List();
        }
    }

    /// <summary>
    /// A player assigned to a team.
    /// </summary>
    public class TeamPlayer
    {
        public virtual int Id { get; set; }
        public virtual Team Team { get; set; }
        public virtual Player Player { get; set; }
        public virtual PlayerStatus Status { get; set; }
        public virtual string Photo { get; set; }
        public virtual string JerseyNumber { get; set; }
        public virtual bool IsPhotoVerified { get; set; }
        public virtual User PhotoVerifiedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string SignWaiverId { get; set; }
        public virtual SignStatus WaiverStatus { get; set; }
        public virtual IList<PlayerGameStat> PlayerGameStats { get; set; }
        
        public static TeamPlayer GetTeamPlayerWithTeamAndPlayer(Team team, Player player)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<TeamPlayer>()
                .Where(m => m.Team == team && m.Player == player)
                .List().FirstOrDefault();
        }
    }
}