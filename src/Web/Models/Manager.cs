using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// A manager of a team.
    /// </summary>
    public class Manager
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

        public static IList<Manager> GetAllManagers()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Manager>().List();
        }

        public static Manager GetManagerById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Manager>(id);
        }

        public static Manager GetManagerById(int id, User user)
        {
            if (user.Role != UserRole.Administrator)
                return null;
            return Manager.GetManagerById(id);
        }

        public static Manager GetManagerWithInviteToken(string token)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Manager>().Where(c => c.InviteToken == token).List().SingleOrDefault();
        }

        public static Manager GetManagerForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Manager>().Where(c => c.User == user).List().SingleOrDefault();
        }

        public static Manager GetManagerForEmail(String email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Manager>().Where(c => c.Email == email).List().FirstOrDefault();
        }
    }
}