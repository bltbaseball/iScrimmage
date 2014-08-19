using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using iScrimmage.Core.Data.Models;
using NHibernate;
using Web.Models;

namespace Web.Transition
{
    public class Membership
    {
        public static void CreateUser(Member member, string role, Contact phone)
        {
            ISession session = MvcApplication.SessionFactory.GetCurrentSession();

            // Create the user in the original membership table - required for the forms authentication
            var user = Models.User.CreateUserFromGoogleOAuth(member.Id.ToString(), member.Email, member.FirstName, member.LastName);

            // Create the associated records based on role
            switch (role)
            {
                case "coach":
                    CreateCoach(session, user, member, phone);
                    break;

                case "guardian":
                    CreateGuardian(session, user, member, phone);
                    break;

                case "playerOver13":
                case "playerUnder13":
                    CreatePlayer(session, user, member, phone);
                    break;

                default:
                    break;
            }
        }

        public static void CreateCoach(ISession session, Web.Models.User user, Member member, Contact phone)
        {
            using (var tx = session.BeginTransaction())
            {
                var coach = new Web.Models.Coach();

                user.Role = UserRole.Coach;
                session.Update(user);

                coach.FirstName = member.FirstName;
                coach.LastName = member.LastName;
                coach.Email = member.Email;
                coach.PhoneNumber = phone.PhoneNumber;
                coach.CreatedOn = DateTime.Now;
                coach.User = user;

                session.Save(coach);

                tx.Commit();
            }
        }

        public static void CreateGuardian(ISession session, Web.Models.User user, Member member, Contact phone)
        {
            using (var tx = session.BeginTransaction())
            {
                var guardian = new Web.Models.Guardian();

                user.Role = UserRole.Guardian;
                session.Update(user);

                guardian.FirstName = member.FirstName;
                guardian.LastName = member.LastName;
                guardian.Email = member.Email;
                guardian.PhoneNumber = phone.PhoneNumber;
                guardian.CreatedOn = DateTime.Now;
                guardian.User = user;

                session.Save(guardian);

                tx.Commit();
            }
        }

        public static void CreatePlayer(ISession session, Web.Models.User user, Member member, Contact phone)
        {
            using (var tx = session.BeginTransaction())
            {
                var player = new Web.Models.Player();

                user.Role = UserRole.Player;
                session.Update(user);

                player.FirstName = member.FirstName;
                player.LastName = member.LastName;
                player.Email = member.Email;
                player.PhoneNumber = phone.PhoneNumber;
                player.Gender = member.Gender == "M" ? Web.Models.Gender.Male : Web.Models.Gender.Female;
                player.DateOfBirth = member.DateOfBirth;
                player.CreatedOn = DateTime.Now;
                player.User = user;

                session.Save(player);

                tx.Commit();
            }
        }
    }
}