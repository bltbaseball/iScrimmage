using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UmpireNewModel
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

        [DisplayName("Lead Umpire of League")]
        public int? LeagueId { get; set; }
    }

    public class UmpireUpdateModel : UmpireNewModel
    {
        public int Id { get; set; }

        public string CurrentPhotoPath { get; set; }
    }

    public class UmpireListModel
    {
        public IList<Umpire> Umpires { get; set; }
    }
}