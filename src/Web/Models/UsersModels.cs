using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UserNewModel
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public UserRole Role { get; set; }
        
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }
    }

    public class UserUpdateModel : UserNewModel
    {
        public int Id { get; set; }
    }
}