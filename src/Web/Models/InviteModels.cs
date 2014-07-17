using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class InviteManagerModel : InviteModel
    {
        
    }

    public class InviteCoachModel : InviteModel
    {

    }

    public class InviteUmpireModel : InviteModel
    {

    }

    public class InvitePlayerModel : InviteModel
    {

    }

    public class InviteGuardianModel : InviteModel
    {

    }

    public class InviteModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class InviteRequestModel
    {
        [Required]
        public UserRole Role { get; set; }

        public string Email { get; set; }

        [DisplayName("First name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string Comments { get; set; }

        public int UserId { get; set; }

        public string RedirectAction { get; set; }
    }

    public class GenerateInviteCreateModel
    {
        [Required]
        public string MessageTo { get; set; }

        [Required]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
    }
}