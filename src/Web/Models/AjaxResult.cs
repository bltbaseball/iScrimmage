using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AjaxResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }

    public class AvailableDatesResponse : AjaxResponse
    {
        public IList<AvailableDateDTO> Dates { get; set; }
    }

    public class LeagueTeamsResponse : AjaxResponse
    {
        public LeagueType LeagueType { get; set; }
        public IList<LeagueTeamDTO> Teams { get; set; }
    }

    public class LeagueTeamDTO
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }

    public class AvailableDateDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
    }

    public class UploadFileResponse : AjaxResponse
    {
        public string ErrorDescription { get; set; }
        public string Type { get; set; }
        public string File { get; set; }
    }
}