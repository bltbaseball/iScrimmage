using iScrimmage.Core.Models;

namespace iScrimmage.Core.Common
{
    public interface IUserSessionProvider
    {
        Member GetCurrentUser();

        bool IsAuthenticated { get; }

        string ServerInstanceName { get; }
    }

    public class UserSessionProvider: IUserSessionProvider
    {
        public Member GetCurrentUser()
        {
            return UserSession.Current != null ? UserSession.Current.User : null;
        }

        public bool IsAuthenticated
        {
            get { return UserSession.Current != null; }
        }

        public string ServerInstanceName
        {
            get 
            { 
                if (IsAuthenticated)
                {
                    var user = GetCurrentUser();
                    //if (user != null && user.CurrentFacility != null)
                    //{
                    //    return user.CurrentFacility.ServerInstanceName;
                    //}
                }

                return null;
            }
        }
    }
}
