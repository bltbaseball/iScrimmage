using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BasicTournamentRegistrationModel
    {
        [DisplayName("Team")]
        public int Id { get; set; }

        [DisplayName("Coach/Manager Name(s)")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Coach/Manager Email")]
        [Required]
        public string Email { get; set; }

        [DisplayName("Coach/Manager Phone Number")]
        [Required]
        public string PhoneNumber { get; set; }

        [DisplayName("Phone Carrier (if using cell phone)")]
        public string PhoneCarrier { get; set; }

        [DisplayName("Team Name")]
        [Required]
        public string Team { get; set; }

        [DisplayName("Age Group")]
        [Required]
        public string AgeGroup { get; set; }

        [DisplayName("Class")]
        [Required]
        public string Class { get; set; }

        [DisplayName("# of Seasons Played")]
        [Required]
        public string SeasonsPlayed { get; set; }
    }


    public class LeagueInfoModel
    {
        public League League { get; set; }
        public Team Team { get; set; }
        public IList<Team> Teams { get; set; }
        public IList<Game> Games { get; set; }
        public IList<LeagueFeeModel> Fees { get; set; }
    }

    public class LeagueFeeModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceId { get; set; }
        public bool IsPaid { get; set; }
    }
}