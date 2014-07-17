using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class CoachNewModel
    {
        [Required]
        public string Email { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        public string Photo { get; set; }

        [DisplayName("Send user invitation to e-mail address")]
        public bool InviteUser { get; set; }

        [DisplayName("Teams")]
        public IEnumerable<int> TeamIds { get; set; }
    }

    public class CoachUpdateModel : CoachNewModel
    {
        public int Id { get; set; }

        public string CurrentPhotoPath { get; set; }
    }

    public class CoachListModel
    {
        public IList<Coach> Coaches { get; set; }
    }
}