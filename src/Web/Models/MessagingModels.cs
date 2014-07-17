using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class MessageCreateModel
    {
        public IList<League> Leagues { get; set; }
        public IList<Team> Teams { get; set; }
        public IList<Player> Players { get; set; }
        public IList<Coach> Coaches { get; set; }
        public IList<Manager> Managers { get; set; }
        public IList<Umpire> Umpires { get; set; }
        public IList<Guardian> Guardians { get; set; }

        [Required]
        public string MessageTo { get; set; }

        [DisplayName("League(s)")]
        public List<int> LeagueIds { get; set; }
        [DisplayName("Team(s)")]
        public List<int> TeamIds { get; set; }
        [DisplayName("Player(s)")]
        public List<int> PlayerIds { get; set; }
        [DisplayName("Coach(es)")]
        public List<int> CoachIds { get; set; }
        [DisplayName("Guardian(s)")]
        public List<int> GuardianIds { get; set; }

        public bool CanMessageLeagues { get; set; }
        public bool CanMessageUmpires { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public IList<MessageLog> SentMessages { get; set; }
    }


}