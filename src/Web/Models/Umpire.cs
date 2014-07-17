using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// An Umpire of a Game.
    /// </summary>
    public class Umpire
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
        public virtual IList<Game> Games { get; set; }
        public virtual League League { get; set; }

        public static IList<Umpire> GetAllUmpires()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Umpire>().List();
        }

        public static Umpire GetUmpireById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Umpire>(id);
        }

        public static Umpire GetUmpireById(int id, User user)
        {
            if (user.Role != UserRole.Administrator)
                return null;
            return Umpire.GetUmpireById(id);
        }

        public static Umpire GetUmpireWithInviteToken(string token)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Umpire>().Where(c => c.InviteToken == token).List().SingleOrDefault();
        }

        public static Umpire GetUmpireForUser(User user)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Umpire>().Where(c => c.User == user).List().SingleOrDefault();
        }

        public static Umpire GetUmpireForEmail(String email)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Umpire>().Where(c => c.Email == email).List().FirstOrDefault();
        }

        public static string GetUmpireName(Umpire umpire)
        {
            if (umpire == null)
                return null;
            return umpire.LastName + ", " + umpire.FirstName;
        }

    }
}