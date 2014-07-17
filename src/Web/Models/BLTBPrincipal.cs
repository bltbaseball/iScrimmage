using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Web.Models
{
    public class BLTBPrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }
        public List<string> Roles { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }

        public bool IsInRole(string role)
        {
            return this.Roles.Contains(role);
        }

        public BLTBPrincipal(IIdentity ident, string name, UserRole role)
        {
            this.Identity = ident;
            this.Roles = new List<string> { role.ToString() };
            this.Name = name;
            this.Role = role;
        }

        public static BLTBPrincipal CreatePrincipal(FormsIdentity id, string email)
        {
            // populate our user principal
            var user = User.GetUserByEmail(email);
            if (user == null)
                return null;
            string name = string.Format("{0} {1}", user.FirstName, user.LastName).Trim();
            var principal = new BLTBPrincipal(id, name, user.Role);
            return principal;
        }
    }
}