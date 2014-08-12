using iScrimmage.Core.Models;
using System;
using System.Web;

namespace iScrimmage.Core.Common
{
    public interface IUserSession
    {
        Member User { get; }

        void Update(Member user);
    }

    public class UserSession
    {
        public const string SessionKey = "iScrimmage User Session";

        public static IUserSession Current
        {
            get
            {
                if (IsHttpSession())
                {
                    return HttpUserSession.Current;
                }

                return InMemoryUserSession.Current;
            }
        }

        public static IUserSession Initialize(Member user)
        {
            var userSession = IsHttpSession() ?
                                    HttpUserSession.Initialize(user) as IUserSession
                                    : InMemoryUserSession.Initialize(user) as IUserSession;
            return userSession;
        }

        private static bool IsHttpSession()
        {
            return HttpContext.Current != null;
        }
    }

    public class HttpUserSession: IUserSession
    {
        public TimeZoneInfo UserTimeZone { get; protected set; }
        public Member User { get; protected set; }

        public HttpUserSession(Member user, TimeZoneInfo tz)
        {
            User = user;
            UserTimeZone = tz ?? TimeZoneInfo.Local;
        }

        public void Update(Member user)
        {
            User = user;

            HttpContext.Current.Session[UserSession.SessionKey] = this;
        }

        public static IUserSession Current
        {
            get
            {
                var httpSession = HttpContext.Current.Session;
                if (httpSession == null)
                {
                    return null;
                }
                
                if (httpSession[UserSession.SessionKey] == null)
                {
                    return null;
                }

                return (HttpUserSession) httpSession[UserSession.SessionKey];
            }
        }

        public static HttpUserSession Initialize(Member user, TimeZoneInfo tz = null)
        {
            var userSession = new HttpUserSession(user, tz);

            HttpContext.Current.Session[UserSession.SessionKey] = userSession;
            return userSession;
        }
    }

    public class InMemoryUserSession: IUserSession
    {
        private static InMemoryUserSession _instance;

        public TimeZoneInfo UserTimeZone { get; protected set; }
        public Member User { get; protected set; }

        public InMemoryUserSession(Member user, TimeZoneInfo tz)
        {
            User = user;
            UserTimeZone = tz ?? TimeZoneInfo.Local;
        }

        public void Update(Member user)
        {
            User = user;
        }

        public static IUserSession Current
        {
            get { return _instance; }
        }

        public static InMemoryUserSession Initialize(Member user, TimeZoneInfo tz = null)
        {
            _instance = new InMemoryUserSession(user, tz);
            return _instance;
        }
    }
}
