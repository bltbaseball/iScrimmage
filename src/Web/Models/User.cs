using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public enum UserRole : int
    {
        Administrator = 0,
        Manager = 1,
        Coach = 2,
        Player = 3,
        Umpire = 4,
        Unknown = 5,
        Guardian = 6
    }

    public class UsersProfile
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual string UserName { get; set; }
    }

    public class User
    {
        public virtual int Id { get; set; }
        //public virtual UsersProfile Profile { get; set; }
        //public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual bool IsConfirmed { get; set; }
        public virtual bool IsEmailConfirmed { get; set; }
        public virtual string ConfirmationToken { get; set; }
        public virtual DateTime? LastPasswordFailureDate { get; set; }
        public virtual int PasswordFailuresSinceLastSuccess { get; set; }
        public virtual DateTime? PasswordChangedDate { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual string PasswordVerificationToken { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        public virtual IList<OAuthMembership> OAuthMemberships { get; set; }
        public virtual IList<MessageLog> MessageLogs { get; set; }

        public static bool IsHeadUmpireOfLeague(System.Security.Principal.IPrincipal user, Game game)
        {
            if (game == null)
                return false;
            if (game.HomeTeam == null)
                return false;
            return IsHeadUmpireOfLeague(user, game.HomeTeam.League);
        }

        public static bool IsHeadUmpireOfLeague(System.Security.Principal.IPrincipal user, Team team)
        {
            if (team == null)
                return false;
            if (team.League == null)
                return false;
            return IsHeadUmpireOfLeague(user, team.League);
        }

        public static bool IsHeadUmpireOfLeague(System.Security.Principal.IPrincipal user, League league)
        {
            if (league == null)
                return false;
            if (user == null)
                return false;
            var localUser = Web.Models.User.GetUserByEmail(user.Identity.Name);
            if (localUser == null)
                return false;
            var umpire = Umpire.GetUmpireForUser(localUser);
            if (umpire == null)
                return false;
            if (umpire.League == null)
                return false;
            if (umpire.League == league)
                return true;
            return false;
        }

        public static bool VerifyUserRole(User user)
        {
            switch (user.Role)
            {
                case UserRole.Administrator:
                    return true;
                case UserRole.Coach:
                    var coach = Coach.GetCoachForUser(user);
                    return (coach != null);
                case UserRole.Guardian:
                    var guardian = Guardian.GetGuardianForUser(user);
                    return (guardian != null);
                case UserRole.Manager:
                    var manager = Manager.GetManagerForUser(user);
                    return (manager != null);
                case UserRole.Player:
                    var player = Player.GetPlayerForUser(user);
                    return (player != null);
                case UserRole.Umpire:
                    var umpire = Umpire.GetUmpireForUser(user);
                    return (umpire != null);
                case UserRole.Unknown:
                    return true;
                default:
                    return false;
            }
        }

        public static void ValidateUserRole(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                var isValid = VerifyUserRole(user);
                if (!isValid)
                {
                    user.Role = UserRole.Unknown;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Cache.Remove("User-" + user.Email);
                    }
                }

                // attempt to automatically link this user up to an existing type by matching its email address
                if (user.Role == UserRole.Unknown)
                {
                    var guardian = Guardian.GetUnlinkedGuardianByEmail(user.Email);
                    var player = Player.GetUnlinkedPlayerByEmail(user.Email);
                    if (guardian != null)
                    {
                        user.Role = UserRole.Guardian;
                        guardian.User = user;
                        session.Update(user);
                        session.Update(guardian);
                    }
                    else if (player != null)
                    {
                        user.Role = UserRole.Player;
                        player.User = user;
                        session.Update(user);
                        session.Update(player);
                    }                    
                }
                tx.Commit();
            }
        }

        public static User GetUserById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<User>(id);
        }

        public static User GetUserByEmail(string email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<User>()
                .Where(u => u.Email == email)
                .List().SingleOrDefault();
        }

        public static User GetUserViaOAuthCredentials(string provider, string providerUserId)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<User>()
                .JoinQueryOver<OAuthMembership>(user => user.OAuthMemberships)
                .Where(membership => membership.Provider == provider && membership.ProviderUserId == providerUserId)
                .List().SingleOrDefault();
        }

        public static void LinkGoogleOAuthToUser(User user, string googleUserId, string firstName, string lastName)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                var oauth = new OAuthMembership();
                oauth.Provider = "google";
                oauth.ProviderUserId = googleUserId;
                oauth.User = user;

                user.FirstName = firstName;
                user.LastName = lastName;
                user.OAuthMemberships = new List<OAuthMembership>();
                user.OAuthMemberships.Add(oauth);

                session.Save(oauth);
                tx.Commit();
            }
        }

        public static void DeleteUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                foreach (var oauth in user.OAuthMemberships)
                {
                    session.Delete(oauth);
                }
                session.Flush(); // I think this is only necessary because of the odd composite key mess with OAuthMembership table
                session.Delete(user);

                tx.Commit();
            }
        }

        public static IList<User> GetAllUsers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.User>().List();
        }

        /// <summary>
        /// When registering, a User must be invited to the system. If not, we don't know what to do with them (Role).
        /// </summary>
        /// <param name="googleUserId"></param>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static User CreateUserFromGoogleOAuth(string googleUserId, string email, string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            if (string.IsNullOrEmpty(googleUserId))
                throw new ArgumentNullException("googleUserId");

            var session = MvcApplication.SessionFactory.GetCurrentSession();
            using (var tx = session.BeginTransaction())
            {
                if (GetUserByEmail(email) != null)
                    throw new ApplicationException("A user with this e-mail address already exists.");

                var user = new User();
                user.Email = email;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.CreateDate = DateTime.Now;
                user.Role = UserRole.Unknown;
                session.Save(user);

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Cache.Remove("User-" + user.Email);
                }

                var oauth = new OAuthMembership();
                oauth.Provider = "google";
                oauth.ProviderUserId = googleUserId;
                oauth.User = user;
                session.Save(oauth);

                // there will really only be one oauth membership associated with a user since we are requiring Google Accounts
                // but we'll keep the feature for now.
                user.OAuthMemberships = new List<OAuthMembership>();
                user.OAuthMemberships.Add(oauth);

                tx.Commit();
                
                return user;
                //OAuthWebSecurity.CreateOrUpdateAccount("google", googleUserId, email);
            }

        }
    }

    public class OAuthMembership
    {
        public virtual User User { get; set; }
        public virtual string Provider { get; set; }
        public virtual string ProviderUserId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as OAuthMembership;
            if (t == null)
                return false;
            if (User == t.User && Provider == t.Provider)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (User.Id + "|" + Provider).GetHashCode();
        }
    }
}