using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Models
{
    public class Member : BaseModel
    {
        public virtual Guid? GuardianId { get; set; }
        public virtual String Email { get; set; }
        public virtual String Password { get; set; }
        public virtual String VerificationToken { get; set; }
        public virtual Boolean EmailVerified { get; set; }
        public virtual String ResetToken { get; set; }
        public virtual DateTime? ResetTokenExpiresOn { get; set; }
        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual String Gender { get; set; }
        public virtual String Photo { get; set; }
        public virtual Boolean LookingForTeam { get; set; }
        public virtual Boolean Umpire { get; set; }
        public virtual Boolean SiteAdmin { get; set; }
        public virtual Int32? OldId { get; set; }

        [NotMapped] 
        public string Role;
    }
}
