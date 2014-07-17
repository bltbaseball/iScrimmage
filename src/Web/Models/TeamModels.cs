using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class TeamListModel
    {
        public IList<TeamListingModel> Teams { get; set; }
    }

    public class TeamListingModel
    {
        public Team Team { get; set; }
        public bool IsPaid { get; set; }
    }

    public class TeamCopyModel
    {
        [DisplayName("Team")]
        public int? TeamId { get; set; }

        [DisplayName("League or Tournament")]
        public int? LeagueId { get; set; }
    }

    public class TeamCreateCopyModel
    {
        public string Choice { get; set; }

        public TeamCopyModel Copy { get; set; }

        public TeamNewModel Create { get; set; }
    }

    public class TeamNewModel
    {
        public string Name { get; set; }
        
        [DisplayName("Team Web Site")]
        public string Url { get; set; }

        [DisplayName("Division")]
        public int? DivisionId { get; set; }

        [DisplayName("Class")]
        public int? ClassId { get; set; }

        [DisplayName("Scrimmage/Tournament/League")]
        public int? LeagueId { get; set; }

        [DisplayName("Default Home Field Location")]
        public int? LocationId { get; set; }

        [DisplayName("Looking for players?")]
        public bool IsLookingForPlayers { get; set; }

        [DisplayName("Dates Available for Games")]
        public IList<AvailableDateNewModel> AvailableDates { get; set; }

        [DisplayName("Team Description")]
        [AllowHtml]
        public string HtmlDescription { get; set; }
    }

    public class AvailableDateNewModel
    {
        [Required]
        public string Date { get; set; }
        [Required]
        public string Time { get; set; }

        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }

        [Required]
        public string Type { get; set; }

        public int? Distance { get; set; }
    }

    public class AvailableDateUpdateModel : AvailableDateNewModel
    {
        public int? Id { get; set; }
    }

    public class TeamUpdateModel : TeamNewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Location")]
        public int LocationId { get; set; }

        [DisplayName("Dates Available for Games")]
        public new IList<AvailableDateUpdateModel> AvailableDates { get; set; }

        public Team Team  { get; set; }
    }

    public class TeamUpdateDatesModel : TeamNewModel
    {
        public int Id { get; set; }

        [DisplayName("Location")]
        public int LocationId { get; set; }

        [DisplayName("Dates Available for Games")]
        public new IList<AvailableDateUpdateModel> AvailableDates { get; set; }

        public Team Team { get; set; }
    }

    public class TeamStatusModel
    {
        public Team Team { get; set; }
        public bool HasMinimumNumberOfPlayers { get; set; }
        public bool HasValidBirthdates { get; set; }
        public bool HasWaiversSigned { get; set; }
        public bool HasPhotosSubmitted { get; set; }
        public bool HasAvailableDatesEntered { get; set; }
        public bool HasPaidMandatoryFees { get; set; }
    }
}