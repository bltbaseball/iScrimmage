using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DivisionNewModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [DisplayName("Max Age")]
        public int MaxAge { get; set; }
    }

    public class DivisionUpdateModel : DivisionNewModel
    {
        public int Id { get; set; }
    }
}