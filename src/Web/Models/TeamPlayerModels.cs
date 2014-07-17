using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TeamPlayerNewModel
    {
        [Required]
        [DisplayName("Team")]
        public int TeamId { get; set; }
        
        [Required]
        [DisplayName("Player")]
        public int PlayerId { get; set; }

        [Required]
        [DisplayName("Photo Verified")]
        public bool IsPhotoVerified { get; set; }

        public string Photo { get; set; }

        public PlayerStatus Status { get; set; }

        public string SignWaiverId { get; set; }

        [Required]
        [DisplayName("Jersey Number")]
        public string JerseyNumber { get; set; }

        [DefaultValue(SignStatus.NotSigned)]
        public SignStatus WaiverStatus { get; set; }
    }

    public class TeamPlayerUpdateModel : TeamPlayerNewModel
    {
        public int Id { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }    
    }

    public class TeamPlayersModel
    {
        public Team Team { get; set; }
        public IList<AvailableDatesDTO> AvailableDates { get; set; }
        public TeamStatusModel Status { get; set; }
        public IList<TeamPlayerDTO> Players { get; set; }
        //public IEnumerable<Web.Models.TeamPlayer> Players { get; set; }
    }
}