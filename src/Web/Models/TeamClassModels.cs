﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TeamClassNewModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [DisplayName("Handicap Points")]
        public int Handicap { get; set; }
    }

    public class TeamClassUpdateModel : TeamClassNewModel
    {
        public int Id { get; set; }
    }
}