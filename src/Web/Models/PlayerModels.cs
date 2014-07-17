using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class PlayerNewModel
    {
        //Used to identify existing player to create a new link
        public int? PlayerId { get; set; }
        public int? UpdatePlayerId { get; set; }

        public int? TeamId { get; set; }

        [DisplayName("Player First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Player Last Name")]
        public string LastName { get; set; }

        [DisplayName("Player Email")]
        public string Email { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Jersey Number")]
        public int JerseyNumber { get; set; }

        [DisplayName("Player Contact Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Player Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [UIHint("ProfileImage")]
        public string ImageUrl { get; set; }

        public int? GuardianId { get; set; }
        public int? UpdateGuardianId { get; set; }

        public GuardianUpdateModel Guardian { get; set; }
    }

    public class PlayerNewUnattachedModel
    {
        //Used to identify existing player to create a new link
        public int? PlayerId { get; set; }
        public int? UpdatePlayerId { get; set; }

        [Required]
        [DisplayName("Player First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Player Last Name")]
        public string LastName { get; set; }

        [DisplayName("Player Email")]
        public string Email { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Jersey Number")]
        public int JerseyNumber { get; set; }

        [DisplayName("Player Contact Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Player Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public int? GuardianId { get; set; }
        public int? UpdateGuardianId { get; set; }

        public GuardianUpdateModel Guardian { get; set; }
    }

    public class GuardianNewModel
    {
        [DisplayName("Guardian First Name")]
        public string FirstName { get; set; }

        [DisplayName("Guardian Last Name")]
        public string LastName { get; set; }

        [DisplayName("Guardian Email")]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Players")]
        public IEnumerable<int> PlayerIds { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    public class GuardianUpdateModel : GuardianNewModel
    {
        public int? Id { get; set; }
    }

    public class PlayerUpdateModel : PlayerNewModel
    {
        public int Id { get; set; }

        public new GuardianUpdateModel Guardian { get; set; }
    }
}