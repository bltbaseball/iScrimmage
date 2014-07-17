using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class LeagueNewModel
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("League Website (url)")]
        public string Url { get; set; }

        [Required]
        [DisplayName("League Start Date")]
        public string StartDate { get; set; }

        [DisplayName("League End Date")]
        public string EndDate { get; set; }

        [Required]
        [DisplayName("Start Registration Date")]
        public string RegistrationStartDate { get; set; }

        [Required]
        [DisplayName("Last Registration Date")]
        public string RegistrationEndDate { get; set; }

        [DisplayName("Roster Lock Date")]
        public string RosterLockedOn { get; set; }

        [Required]
        [DisplayName("Is League Active?")]
        public bool IsActive { get; set; }

        [Required]
        [DisplayName("Electronic Waiver?")]
        public bool WaiverRequired { get; set; }

        [DisplayName("HTML Description")]
        [AllowHtml]
        public string HtmlDescription { get; set; }

        [Required]
        public LeagueType Type { get; set; }

        [DisplayName("Minimum number of available dates per team")]
        public int? MinimumDatesAvailable { get; set; }

        [DisplayName("Fees")]
        public IList<FeeNewModel> Fees { get; set; }

        public IList<Division> Divisions { get; set; }

        public IList<Team> Teams { get; set; }
    }

    public class FeeNewModel
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Mandatory?")]
        public bool IsRequired { get; set; }
    }

    public class FeeEditModel : FeeNewModel
    {
        public int? Id { get; set; }
    }

    public class LeagueUpdateModel : LeagueNewModel
    {
        public int Id { get; set; }

        [DisplayName("Fees")]
        public new IList<FeeEditModel> Fees { get; set; }
    }
}